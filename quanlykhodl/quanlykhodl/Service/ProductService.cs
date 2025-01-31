using AutoMapper;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using Twilio.Rest.Trunking.V1;

namespace quanlykhodl.Service
{
    public class ProductService : IProductService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public ProductService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<ProductDTO>> Add(ProductDTO productDTO)
        {
            try
            {
                var user = _userService.name();
                var checkCategory = _context.categories.Where(x => x.id == productDTO.category_map && !x.Deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == productDTO.suppliers && !x.Deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.id == productDTO.areaId && !x.Deleted).FirstOrDefault();
                var checkName = _context.products1.Where(x => x.title == productDTO.title && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();

                if (checkName != null || checkCategory == null 
                    || checkSupplier == null || checkArea == null 
                    || checkAccount == null)
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATATONTAI));

                if (!checkLocationExsis(checkCategory.id, checkArea.id, productDTO.location))
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.NOCATEGORY));

                if (!checkLocalFull(checkArea, productDTO.location.Value, productDTO.quantityArea.Value))
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.FULLQUANTITY));

                var mapData = _mapper.Map<product>(productDTO);
                mapData.account = checkAccount;
                mapData.account_map = checkAccount.id;
                mapData.suppliers = checkSupplier.id;
                mapData.supplier_id = checkSupplier;
                mapData.categoryid123 = checkCategory;
                mapData.category_map = checkCategory.id;
                mapData.code = RanDomCode.geneAction(8);

                _context.products1.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.products1.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if(productDTO.images != null)
                {
                    UploadImage(productDTO.images, dataNew);
                }

                var locationProductArea = new productlocation
                {
                    areas = checkArea,
                    id_area = checkArea.id,
                    id_product = dataNew.id,
                    location = productDTO.location.Value,
                    products = dataNew,
                    quantity = productDTO.quantityArea.Value,
                    isAction = true
                };

                _context.productlocations.Add(locationProductArea);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductDTO>.Successfully(productDTO));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkLocalFull(Area area, int localtion, int quantity)
        {
            var checkLocation = _context.locationExceptions.Where(x => x.id_area == area.id && x.location == localtion && !x.Deleted).FirstOrDefault();
            var checkProductLocation = _context.productlocations.Where(x => x.id_area == area.id && x.location == localtion && !x.Deleted && x.isAction).Sum(x => x.quantity);
            if (checkLocation != null)
            {
                
                if(checkLocation.max < checkProductLocation + quantity)
                {
                    return false;
                }
            }
            else
            {
                var checkArea = _context.areas.Where(x => x.id == area.id && !x.Deleted).FirstOrDefault();
                if(checkArea != null)
                {
                    if (checkArea.max < checkProductLocation + quantity)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool  checkLocationExsis(int idCategory, int area, int? location)
        {
            var checkId = _context.productlocations.Where(x => x.id_area == area && x.location == location && !x.Deleted && x.isAction).ToList();

            if (checkId == null)
                return false;
            foreach(var item in checkId)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                if (checkProduct.category_map != idCategory)
                    return false;
            }

            return true;
        }

        private void UploadImage(List<IFormFile> data, product productId)
        {
            var list = new List<ImageProduct>();
            foreach(var item in data)
            {
                uploadCloud.CloudInaryIFromAccount(item, TokenViewModel.PRODUCT + productId.id.ToString(), _cloud);
                var imageProduct = new ImageProduct
                {
                    Link = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    productMap = productId.id,
                    products_id = productId
                };

                list.Add(imageProduct);
            }

            _context.imageProducts.AddRange(list);
            _context.SaveChanges();
        }

        public async Task<PayLoad<ProductAddAreas>> AddArea(ProductAddAreas productDTO)
        {
            try
            {
                
                var checkProduct = _context.products1.Where(x => x.id == productDTO.id_product && !x.Deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.id == productDTO.id_area && !x.Deleted).FirstOrDefault();
                var totalQuantityProduct = _context.productlocations.Where(x => x.id_product == checkProduct.id && !x.Deleted && x.isAction).Sum(x => x.quantity);

                if(checkArea == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));
                if (checkArea.quantity < productDTO.location)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.ERRORLOCATION));

                if (checkProduct == null && checkArea == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                if (checkProduct.quantity < (productDTO.quantity + totalQuantityProduct))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                if (!checkLocationExsis(checkProduct.category_map.Value, checkArea.id, productDTO.location))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.NOCATEGORY));

                if(!checkLocationExsisArea(checkArea, productDTO.location, productDTO.quantity))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                var checkPoductExsis = _context.productlocations.Where(x => x.location == productDTO.location && x.id_area == checkArea.id && x.id_product == checkProduct.id && !x.Deleted && x.isAction).FirstOrDefault();
                if(checkPoductExsis != null)
                {
                    checkPoductExsis.quantity += productDTO.quantity;
                    _context.productlocations.Update(checkPoductExsis);
                }
                else
                {
                    var productLocationArea = new productlocation
                    {
                        areas = checkArea,
                        id_area = checkArea.id,
                        id_product = checkProduct.id,
                        location = productDTO.location,
                        quantity = productDTO.quantity,
                        products = checkProduct,
                        isAction = true
                    };

                    _context.productlocations.Add(productLocationArea);
                }
                
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductAddAreas>.Successfully(productDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(ex.Message));
            }
        }

        private bool checkLocationExsisArea(Area area, int location, int quantity)
        {
            var checkLocation = _context.locationExceptions.Where(x => x.id_area == area.id && x.location == location && !x.Deleted).FirstOrDefault();
            var checkProductLocation = _context.productlocations.Where(x => x.id_area == area.id && x.location == location && !x.Deleted).Sum(x => x.quantity);

            if (checkLocation != null)
            {
                if (checkLocation.max < (checkProductLocation + quantity))
                    return false;
            }
            else
            {
                if (area.max < (checkProductLocation + quantity))
                    return false;
            }
            return true;
        }

        public async Task<PayLoad<bool>> checkLocation(checkLocation data)
        {
            try
            {
                var checkProduct = _context.products1.Where(x => x.id == data.id_product && !x.Deleted).FirstOrDefault();

                if(checkProduct == null)
                    return await Task.FromResult(PayLoad<bool>.CreatedFail(Status.DATANULL));

                if (!checkLocationExsis(checkProduct.category_map.Value, data.id_Area, data.location))
                    return await Task.FromResult(PayLoad<bool>.CreatedFail(Status.NOCATEGORY));

                return await Task.FromResult(PayLoad<bool>.Successfully(true));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<bool>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.products1.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.Deleted = true;

                _context.products1.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkdata = _context.products1.Where(x => !x.Deleted).ToList();

                if(!string.IsNullOrEmpty(name))
                    checkdata = checkdata.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadDataCategory(checkdata), page - 1, pageSize);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneByArea(int id)
        {
            try
            {
                var checkArea = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();

                if (checkArea == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(findAreaproduct(checkArea)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private productArea findAreaproduct(Area area)
        {
            var data = new productArea();
            data.Id = area.id;
            data.quantity = area.quantity.Value;
            data.name = area.name;
            data.totalLocation = totalQuantityLocation(area);
            data.totalLocationEmpty = CheckLocationUsed(area);
            data.totalLocatiEmpty = totalQuantityLocation(area) - CheckLocationUsed(area);
            data.productLocationAreas = productLocationAreas(area.id);
            data.productPlans = productLocationAreasPlan(area.id);
            data.locationTotal = checkLocation(area.id);
            data.warehoursPlans = loadDataWarehoursePlan(area.id);
            data.totalLocatiExsis = area.quantity.Value - checkLocationAreaExsis(area);

            return data;
        }

        private int checkLocationUsed(Area area, int location)
        {
            var checkData = _context.productlocations.Where(x => x.id_area == area.id && x.location == location && !x.Deleted && x.isAction).Sum(x => x.quantity);
            return checkData;
        }
        private int CheckLocationUsed(Area area)
        {
            int sum = 0;

            for (var i = 1; i <= area.quantity; i++)
            {
                sum += checkLocationUsed(area, i);
            }
            return sum;
        }

        private int checkLocationAreaExsis(Area area)
        {
            var list = new List<int>();

            var checkLocationTotal = _context.productlocations.Where(x => x.id_area == area.id && !x.Deleted).ToList();
            if (checkLocationTotal.Count > 0)
            {
                foreach (var item in checkLocationTotal)
                {
                    if (!list.Contains(item.location))
                        list.Add(item.location);
                }

            }
            return list.Count();
        }
        private List<WarehoursPlan> loadDataWarehoursePlan(int id)
        {
            var list = new List<WarehoursPlan>();

            var checkPlan = _context.plans.Where(x => x.area == id && x.isWarehourse && !x.Deleted).ToList();
            if(checkPlan != null && checkPlan.Any())
            {
                foreach(var item in checkPlan)
                {
                    var DataItem = new WarehoursPlan();
                    var checkArea = _context.areas.Where(x => x.id == item.area && !x.Deleted).FirstOrDefault();
                    if(checkArea != null)
                    {
                        DataItem.area = checkArea.name;
                        var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.Deleted).FirstOrDefault();
                        if(checkFloor != null)
                        {
                            DataItem.floor = checkFloor.name;
                            var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.Deleted).FirstOrDefault();
                            if (checkWarehourse != null)
                                DataItem.warehours = checkWarehourse.name;
                        }
                    }

                    list.Add(DataItem);
                }
                
            }

            return list;
        }
        private int totalLocationEmty(Area area)
        {
            var checkTotal = _context.productlocations.Where(x => x.id_area == area.id && !x.Deleted && x.isAction).Sum(x => x.quantity);

            return checkTotal;
        }
        private int? checkQuantityEmty(Area area)
        {
            int? sum = 0;
            
            for(var i = 1; i <= area.quantity; i++)
            {
                if(checkAreaLocationExsis(area, i))
                {
                    sum += quantityArea(area, i);
                }
            }
            return sum;
        }

        private bool checkAreaLocationExsis(Area area, int location)
        {
            var checkDataProductLocation = _context.productlocations.Where(x => x.id_area == area.id && x.location == location && !x.Deleted && x.isAction).FirstOrDefault();
            if (checkDataProductLocation != null)
                return false;
            return true;
        }

        private int? quantityArea(Area area, int location)
        {
            var checkLocation = _context.locationExceptions.Where(x => x.id_area == area.id && x.location == location && !x.Deleted).FirstOrDefault();
            if (checkLocation != null)
            {
                return checkLocation.max;
            }
            else
            {
                var checkLocationArea = _context.areas.Where(x => x.id == area.id && !x.Deleted).FirstOrDefault();
                if(checkLocationArea != null)
                    return checkLocationArea.max;
            }
            return 0;
        }

        private int totalQuantityLocation(Area data)
        {
            var checkLocationExceps = _context.locationExceptions.Where(x => x.id_area == data.id && !x.Deleted).Count();
            var total = data.quantity - checkLocationExceps;

            var totalNoExCeps = (total * data.max) + totalLocal(data.id);

            return totalNoExCeps.Value;
        }

        private int totalLocal(int id)
        {
            var checkArea = _context.locationExceptions.Where(x => x.id_area == id && !x.Deleted).Sum(x => x.max);

            return checkArea.Value;
        }
        private Dictionary<int, int> checkLocation(int id)
        {
            var dictionary = new Dictionary<int, int>();
            //var checkPlan = _context.plans.Where(x => x.area == id && !x.isWarehourse).ToList();
            var checkPlan = _context.plans.Where(x => x.area == id).ToList();
            if (checkPlan != null && checkPlan.Any())
            {
                foreach(var item in checkPlan)   
                {
                    if (!dictionary.ContainsKey(item.localtionNew.Value))
                    {
                        var checkLocation = _context.plans.Where(x => x.area == id && x.localtionNew == item.localtionNew).Count();
                        dictionary.Add(item.localtionNew.Value, checkLocation);
                    }
                }
            }
            return dictionary;
        }

        private List<productLocationArea> productLocationAreas(int id)
        {
            var list = new List<productLocationArea>();
            var checkData = _context.productlocations.Where(x => x.id_area == id && !x.Deleted && x.isAction).ToList();

            foreach(var item in checkData)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                if (checkProduct != null)
                {
                    var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.Deleted).FirstOrDefault();
                    var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.Deleted).FirstOrDefault();
                    var checkAccountCreate = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();
                    var imageProductData = _context.imageProducts.Where(x => x.productMap == checkProduct.id && !x.Deleted).ToList();
                    var checkLocationCode = _context.codelocations.Where(x => x.id_area == id && x.location == item.location && !x.Deleted).FirstOrDefault();
                    var dataItem = new productLocationArea
                    {
                        Id = item.id,
                        Id_product = checkProduct.id,
                        name = checkProduct.title,
                        image = imageProductData[0].Link,
                        images = imageProductData.Select(x => x.Link),
                        location = item.location,
                        quantity = item.quantity,
                        supplier = checkSupplier.name,
                        supplier_image = checkSupplier.image,
                        account_image = checkAccountCreate.image,
                        account_name = checkAccountCreate.username,
                        category = checkCategory.name,
                        category_image = checkCategory.image,
                        code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code,
                        Inventory = checkProduct.quantity,
                        price = checkProduct.price
                    };
                    list.Add(dataItem);
                }
            }
            return list;
        }
        private List<productLocationArea> productLocationAreasPlan(int id)
        {
            var list = new List<productLocationArea>();

            var checkPlan = _context.plans.Where(x => x.area == id && !x.Deleted && x.status.ToLower() != Status.DONE.ToLower()).ToList();
            if (checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    var checkLocationProduct = _context.productlocations.Where(x => x.location == item.localtionOld && x.id_area == item.areaOld && !x.Deleted && x.isAction).ToList();
                    if (checkLocationProduct != null && checkLocationProduct.Count > 0)
                    {
                        foreach (var itemPlan in checkLocationProduct)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == itemPlan.id_product && !x.Deleted).FirstOrDefault();
                            if (checkProduct != null)
                            {
                                var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.Deleted).FirstOrDefault();
                                var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.Deleted).FirstOrDefault();
                                var checkAccountCreate = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();
                                var imageProductData = _context.imageProducts.Where(x => x.productMap == checkProduct.id && !x.Deleted).ToList();
                                var checkLocationCode = _context.codelocations.Where(x => x.id_area == id && x.location == item.localtionNew && !x.Deleted).FirstOrDefault();
                                var dataItem = new productLocationArea
                                {
                                    Id_product = checkProduct.id,
                                    location = item.localtionNew,
                                    Id = itemPlan.id,
                                    image = imageProductData[0].Link,
                                    images = imageProductData.Select(x => x.Link),
                                    name = checkProduct.title,
                                    quantity = itemPlan.quantity,
                                    Id_plan = item.id,
                                    supplier = checkSupplier.name,
                                    supplier_image = checkSupplier.image,
                                    account_image = checkAccountCreate.image,
                                    account_name = checkAccountCreate.username,
                                    category = checkCategory.name,
                                    category_image = checkCategory.image,
                                    code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code,
                                    Inventory = checkProduct.quantity,
                                    price = checkProduct.price
                                };

                                list.Add(dataItem);
                            }
                        }
                    }
                }
            }

            return list;
        }
        //private List<productLocationArea> productLocationAreasPlan(int id)
        //{
        //    var list = new List<productLocationArea>();

        //    var checkPlan = _context.plans.Where(x => x.area == id && !x.Deleted && x.status.ToLower() != Status.DONE.ToLower()).ToList();
        //    if (checkPlan.Any())
        //    {
        //        foreach (var item in checkPlan)
        //        {
        //            var checkLocationProduct = _context.productlocations.Where(x => x.location == item.localtionNew && x.id_area == item.area && !x.Deleted).FirstOrDefault();
        //            var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.Deleted).FirstOrDefault();
        //            var imageProductData = _context.imageProducts.Where(x => x.productMap == checkProduct.id && !x.Deleted).FirstOrDefault();
        //            var checkLocationCode = _context.codelocations.Where(x => x.id_area == id && x.location == item.localtionNew && !x.Deleted).FirstOrDefault();
        //            var dataItem = new productLocationArea
        //            {
        //                Id_product = checkProduct.id,
        //                location = item.localtionNew,
        //                Id = checkLocationProduct.id,
        //                image = imageProductData.Link,
        //                name = checkProduct.title,
        //                quantity = checkLocationProduct.quantity,
        //                Id_plan = item.id,
        //                code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code
        //            };

        //            list.Add(dataItem);
        //        }
        //    }

        //    return list;
        //}

        public async Task<PayLoad<object>> FindOneByCategory(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkIdCategory = _context.products1.Where(x => x.category_map == id && !x.Deleted).ToList();

                if (checkIdCategory == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var pageList = new PageList<object>(loadDataCategory(checkIdCategory), page - 1, pageSize);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<ProductGetAll> loadDataCategory(List<product> data)
        {
            var list = new List<ProductGetAll>();

            if (data.Any())
            {
                foreach(var item in data)
                {
                    var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.Deleted).FirstOrDefault();
                    var mapData = _mapper.Map<ProductGetAll>(item);
                    mapData.categoryImage = checkCategory == null ? Status.NOCATEGORY : checkCategory.image;
                    mapData.categoryName = checkCategory == null ? Status.NOCATEGORY : checkCategory.name;
                    mapData.images = ListImage(item.id);
                    mapData.listAreaOfproducts = loadDataAreaAndFloorAndWerahourse(item.id);

                    list.Add(mapData);
                }
            }

            return list;
        }

        public async Task<PayLoad<ProductGetAll>> FindOneById(int id)
        {
            try {
                var checkId = _context.products1.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<ProductGetAll>.CreatedFail(Status.DATANULL));

                var checkCategory = _context.categories.Where(x => x.id == checkId.category_map && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<ProductGetAll>(checkId);
                if(checkCategory != null)
                {
                    mapData.categoryImage = checkCategory.image;
                    mapData.categoryName= checkCategory.name;
                }
                mapData.images = ListImage(checkId.id);
                mapData.listAreaOfproducts = loadDataAreaAndFloorAndWerahourse(checkId.id);

                return await Task.FromResult(PayLoad<ProductGetAll>.Successfully(mapData));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductGetAll>.CreatedFail(ex.Message));
            }
        }

        private List<listAreaOfproduct> loadDataAreaAndFloorAndWerahourse(int id)
        {
            var list = new List<listAreaOfproduct>();
            var checkProductLocation = _context.productlocations.Where(x => x.id_product == id && !x.Deleted && x.isAction).ToList();
            if (checkProductLocation.Any())
            {
                foreach (var item in checkProductLocation)
                {
                    var checkArea = _context.areas.Where(x => x.id == item.id_area && !x.Deleted).FirstOrDefault();
                    var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.Deleted).FirstOrDefault();
                    var checkLocationAreaCode = _context.codelocations.Where(x => x.id_area == checkArea.id && x.location == item.location && !x.Deleted).FirstOrDefault();
                    var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.Deleted).FirstOrDefault();
                    var checkAccount = _context.accounts.Where(x => x.id == checkWarehourse.account_map && !x.Deleted).FirstOrDefault();
                    var dataItem = new listAreaOfproduct
                    {
                        account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image,
                        account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username,
                        addressWarehouse = checkWarehourse.Country + checkWarehourse.City + checkWarehourse.District + checkWarehourse.Street,
                        warehouse_name = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.name,
                        warehouse_image = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.image,
                        floor_image = checkFloor == null ? Status.NOFLOOR : checkFloor.image,
                        floor_name = checkFloor == null ? Status.NOFLOOR : checkFloor.name,
                        area_image = checkArea == null ? Status.NOAREA : checkArea.image,
                        area_name = checkArea == null ? Status.NOAREA : checkArea.name,
                        quantity = item.quantity,
                        Id_productlocation = item.id,
                        location = item.location,
                        idArea = checkArea.id,
                        MaxlocationExceps = QuantityAreaMax(checkArea.id, item.location),
                        MaxlocationArea = checkArea.max,
                        code = checkLocationAreaCode == null ? Status.CODEFAILD : checkLocationAreaCode.code
                    };

                    list.Add(dataItem);
                }
            }

            return list;
        }

        private int QuantityAreaMax(int id, int location)
        {
            var checkProductExceps = _context.locationExceptions.Where(x => x.id_area == id && !x.Deleted && x.location == location).FirstOrDefault();
            if(checkProductExceps != null)
            {
                return checkProductExceps.max.Value;
            }
            else
            {
                var checkArea = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkArea != null)
                    return checkArea.max.Value;
            }
            return 0;
        }
        private List<string> ListImage(int id)
        {
            var list = new List<string>();

            var checkProduct = _context.imageProducts.Where(x => x.productMap == id && !x.Deleted).ToList();
            if (checkProduct.Any())
            {
                foreach(var item in checkProduct)
                {
                    list.Add(item.Link);
                }
            }
            return list;
        }
        public async Task<PayLoad<ProductDTO>> Update(int id, ProductDTO productDTO)
        {
            try
            {
                var checkId = _context.products1.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                var checkName = _context.products1.Where(x => x.title == productDTO.title && !x.Deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == productDTO.suppliers && !x.Deleted).FirstOrDefault();
                var checkCategoty = _context.categories.Where(x => x.id == productDTO.category_map && !x.Deleted).FirstOrDefault();
                
                if (checkId == null || checkSupplier == null || checkCategoty == null || checkName != null)
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATANULL));
                if (productDTO.images != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.PRODUCT + checkId.id.ToString(), _cloud);
                    var checkImageProduct = _context.imageProducts.Where(x => x.productMap == checkId.id && !x.Deleted).ToList();
                    if(checkImageProduct.Any())
                    {
                        _context.imageProducts.RemoveRange(checkImageProduct);
                        _context.SaveChanges();
                    }

                    UploadImage(productDTO.images, checkId);
                }

                var mapDataUpdate = MapperData.GanData(checkId, productDTO);
                mapDataUpdate.suppliers = checkSupplier.id;
                mapDataUpdate.supplier_id = checkSupplier;
                mapDataUpdate.categoryid123 = checkCategoty;
                mapDataUpdate.category_map = checkCategoty.id;
                mapDataUpdate.UpdatedAt = DateTimeOffset.UtcNow;

                _context.products1.Update(mapDataUpdate);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductDTO>.Successfully(productDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneBySipplier(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.products1.Where(x => x.suppliers == id && !x.Deleted).ToList();

                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var pageList = new PageList<object>(loadDataCategory(checkData), page - 1, pageSize);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ProductAddAreas>> UpdateArea(int id, ProductAddAreas productDTO)
        {
            try
            {
                var checkLocationProduct = _context.productlocations.Where(x => x.id == id && !x.Deleted && x.isAction).FirstOrDefault();
                if (checkLocationProduct == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.Deleted).FirstOrDefault();
                var checkTotalQuantity = _context.productlocations.Where(x => x.id_product == checkProduct.id && x.id != checkLocationProduct.id && !x.Deleted && x.isAction).Sum(x => x.quantity);
                if (checkProduct.quantity < checkTotalQuantity + productDTO.quantity)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                var checkArea = _context.areas.Where(x => x.id == productDTO.id_area && !x.Deleted).FirstOrDefault();
                if(checkArea == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                if (!checkLocationExsisArea(checkArea, productDTO.location, productDTO.quantity))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                checkLocationProduct.quantity += productDTO.quantity;

                _context.productlocations.Update(checkLocationProduct);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductAddAreas>.Successfully(productDTO));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneProductInWarehourse(int id)
        {
            try
            {
                var checkId = _context.productlocations.Where(x => x.id == id && !x.Deleted && x.isAction).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkProduct = _context.products1.Where(x => x.id == checkId.id_product && !x.Deleted).FirstOrDefault();
                if (checkProduct == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                var checkArea = _context.areas.Where(x => x.id == checkId.id_area && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == checkId.id_area && !x.Deleted).FirstOrDefault();
                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();
                var checkLocationCode = _context.codelocations.Where(x => x.location == checkId.location && x.id_area == checkArea.id).FirstOrDefault();

                if (checkArea == null || checkFloor == null || checkWarehourse == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<ProductOneLocation>(checkProduct);
                mapData.Id = checkId.id;
                mapData.Id_product = checkProduct.id;
                mapData.images = ListImage(checkId.id);
                mapData.account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
                mapData.account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
                mapData.warehouse_image = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.image;
                mapData.warehouse_name = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.name;
                mapData.floor_name = checkFloor == null ? Status.NOFLOOR : checkFloor.name;
                mapData.floor_image = checkFloor == null ? Status.NOFLOOR : checkFloor.image;
                mapData.area_image = checkArea == null ? Status.NOAREA : checkArea.image;
                mapData.area_name = checkArea == null ? Status.NOAREA : checkArea.image;
                mapData.code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code;
                mapData.TotalLocationEmty = checkQuantityEmty(checkArea);
                return await Task.FromResult(PayLoad<object>.Successfully(mapData));
                
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllProductInWarehourse(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.productlocations.Where(x => !x.Deleted && x.isAction).ToList();
                var dataMap = loadDataProductInLocation(data);
                if (!string.IsNullOrEmpty(name))
                    dataMap = dataMap.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(dataMap, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<ProductOneLocation> loadDataProductInLocation(List<productlocation> data)
        {
            var list = new List<ProductOneLocation>();

            if(data != null)
            {
                foreach(var item in data)
                {
                    var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                    if(checkProduct != null)
                    {
                        var mapData = _mapper.Map<ProductOneLocation>(checkProduct);
                        var checkArea = _context.areas.Where(x => x.id == item.id_area && !x.Deleted).FirstOrDefault();
                        if(checkArea != null)
                        {
                            var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.Deleted).FirstOrDefault();
                            if(checkFloor != null)
                            {
                                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.Deleted).FirstOrDefault();
                                if(checkWarehourse != null)
                                {
                                    var checkLocationCode = _context.codelocations.Where(x => x.location == item.location && x.id_area == checkArea.id && !x.Deleted).FirstOrDefault();
                                    mapData.warehouse_image = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.image;
                                    mapData.warehouse_name = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.name;
                                    mapData.floor_image = checkFloor == null ? Status.NOFLOOR : checkFloor.image;
                                    mapData.floor_name = checkFloor == null ? Status.NOFLOOR : checkFloor.name;
                                    mapData.code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code;
                                    mapData.TotalLocationEmty = checkQuantityEmty(checkArea);
                                }
                            }
                        }
                        mapData.images = ListImage(checkProduct.id);
                        mapData.Id = item.id;
                        mapData.Id_product = checkProduct.id;
                        mapData.area_image = checkArea == null ? Status.NOAREA : checkArea.image;
                        mapData.area_name = checkArea == null ? Status.NOAREA : checkArea.name;
                        
                        list.Add(mapData);
                    }
                    
                }
            }
            return list;
        }

        public async Task<PayLoad<object>> FindCode(string code)
        {
            try
            {
                var checkCode = _context.products1.Where(x => x.code == code && !x.Deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkCategory = _context.categories.Where(x => x.id == checkCode.category_map && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<ProductGetAll>(checkCode);
                if (checkCategory != null)
                {
                    mapData.categoryImage = checkCategory.image;
                    mapData.categoryName = checkCategory.name;
                }
                mapData.images = ListImage(checkCode.id);
                mapData.listAreaOfproducts = loadDataAreaAndFloorAndWerahourse(checkCode.id);

                return await Task.FromResult(PayLoad<object>.Successfully(mapData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindCodeLocation(string code)
        {
            try
            {
                var checkCode = _context.codelocations.Where(x => x.code == code && !x.Deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkLocationProduct = _context.productlocations.Where(x => x.id_area == checkCode.id_area && x.location == checkCode.location && !x.Deleted && x.isAction).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(loadDataProductInLocation(checkLocationProduct)));
                
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<bool>> checkLocationTotal(checkLocationExsis data)
        {
            try
            {
                var checkLocation = _context.productlocations.Where(x => x.id_area == data.id_Area && x.location == data.location && !x.Deleted && x.isAction).ToList();
                if (checkLocation.Count > 0)
                    return await Task.FromResult(PayLoad<bool>.Successfully(true));
                return await Task.FromResult(PayLoad<bool>.Successfully(false));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<bool>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ProductAddAreas>> UpdateAreaQuantity(int id, ProductAddAreas productDTO)
        {
            try
            {
                var checkProductLocation = _context.productlocations.Where(x => x.id == id && !x.Deleted && x.isAction).FirstOrDefault();
                if (checkProductLocation == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                if (checkProductLocation.quantity < productDTO.quantity)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                checkProductLocation.quantity -= productDTO.quantity;
                //if (checkProductLocation.quantity - productDTO.quantity == 0)
                //    checkProductLocation.Deleted = true;
                _context.productlocations.Update(checkProductLocation);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductAddAreas>.Successfully(productDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(ex.Message));
            }
        }
    }
}