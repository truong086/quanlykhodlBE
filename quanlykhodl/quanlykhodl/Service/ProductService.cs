using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class ProductService : IProductService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        private List<productWarehouse> productWarehousesData;
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
                var checkCategory = _context.categories.Where(x => x.id == productDTO.category_map && !x.deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == productDTO.suppliers && !x.deleted).FirstOrDefault();
                var checkShelf = _context.shelfs.Where(x => x.id == productDTO.shelfId && !x.deleted).FirstOrDefault();
                var checkName = _context.products1.Where(x => x.title == productDTO.title && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();

                if (checkName != null || checkCategory == null 
                    || checkSupplier == null || checkShelf == null 
                    || checkAccount == null)
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATATONTAI));

                if (!checkLocationExsis(checkCategory.id, checkShelf.id, productDTO.location))
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.NOCATEGORY));

                if (!checkLocalFull(checkShelf, productDTO.location.Value, productDTO.quantityshelf.Value))
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.FULLQUANTITY));

                var mapData = _mapper.Map<product>(productDTO);
                mapData.account = checkAccount;
                mapData.account_map = checkAccount.id;
                mapData.suppliers = checkSupplier.id;
                mapData.supplier_id = checkSupplier;
                mapData.categoryid123 = checkCategory;
                mapData.category_map = checkCategory.id;

                _context.products1.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.products1.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                if(productDTO.images != null)
                {
                    UploadImage(productDTO.images, dataNew);
                }

                var locationProductArea = new productlocation
                {
                    shelfs = checkShelf,
                    id_shelf = checkShelf.id,
                    id_product = dataNew.id,
                    location = productDTO.location.Value,
                    products = dataNew,
                    quantity = productDTO.quantityshelf.Value,
                    isaction = true
                };
                dataNew.code = RanDomCode.geneAction(8) + dataNew.id.ToString();

                _context.products1.Update(dataNew);
                _context.productlocations.Add(locationProductArea);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductDTO>.Successfully(productDTO));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkLocalFull(Shelf shelf, int localtion, int quantity)
        {
            var checkLocation = _context.locationexceptions.Where(x => x.id_shelf == shelf.id && x.location == localtion && !x.deleted).FirstOrDefault();
            var checkProductLocation = _context.productlocations.Where(x => x.id_shelf == shelf.id && x.location == localtion && !x.deleted && x.isaction).Sum(x => x.quantity);
            if (checkLocation != null)
            {
                
                if(checkLocation.max < checkProductLocation + quantity)
                {
                    return false;
                }
            }
            else
            {
                var checkArea = _context.shelfs.Where(x => x.id == shelf.id && !x.deleted).FirstOrDefault();
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

        private bool  checkLocationExsis(int idCategory, int shelf, int? location)
        {
            var checkId = _context.productlocations.Where(x => x.id_shelf == shelf && x.location == location && !x.deleted && x.isaction).ToList();

            if (checkId == null)
                return false;
            foreach(var item in checkId)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
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
                    link = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    productmap = productId.id,
                    products_id = productId
                };

                list.Add(imageProduct);
            }

            _context.imageproducts.AddRange(list);
            _context.SaveChanges();
        }

        public async Task<PayLoad<ProductAddAreas>> AddArea(ProductAddAreas productDTO)
        {
            try
            {
                
                var checkProduct = _context.products1.Where(x => x.id == productDTO.id_product && !x.deleted).FirstOrDefault();
                if(checkProduct == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                var checkShelf = _context.shelfs.Where(x => x.id == productDTO.id_shefl && !x.deleted).FirstOrDefault();
                if(checkShelf == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                var totalQuantityProduct = _context.productlocations.Where(x => x.id_product == checkProduct.id && !x.deleted && x.isaction).Sum(x => x.quantity);

                if(checkShelf == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));
                if (checkShelf.quantity < productDTO.location)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.ERRORLOCATION));

                if (checkProduct == null && checkShelf == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                if (checkProduct.quantity < (productDTO.quantity + totalQuantityProduct))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                if (!checkLocationExsis(checkProduct.category_map.Value, checkShelf.id, productDTO.location))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.NOCATEGORY));

                if(!checkLocationExsisArea(checkShelf, productDTO.location, productDTO.quantity))
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                var checkPoductExsis = _context.productlocations.Where(x => x.location == productDTO.location && x.id_shelf == checkShelf.id && x.id_product == checkProduct.id && !x.deleted && x.isaction).FirstOrDefault();
                if(checkPoductExsis != null)
                {
                    checkPoductExsis.quantity += productDTO.quantity;
                    _context.productlocations.Update(checkPoductExsis);
                }
                else
                {
                    var productLocationArea = new productlocation
                    {
                        shelfs = checkShelf,
                        id_shelf = checkShelf.id,
                        id_product = checkProduct.id,
                        location = productDTO.location,
                        quantity = productDTO.quantity,
                        products = checkProduct,
                        isaction = true
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

        private bool checkLocationExsisArea(Shelf shelf, int location, int quantity)
        {
            var checkLocation = _context.locationexceptions.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted).FirstOrDefault();
            var checkProductLocation = _context.productlocations.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted).Sum(x => x.quantity);

            if (checkLocation != null)
            {
                if (checkLocation.max < (checkProductLocation + quantity))
                    return false;
            }
            else
            {
                if (shelf.max < (checkProductLocation + quantity))
                    return false;
            }
            return true;
        }

        public async Task<PayLoad<bool>> checkLocation(checkLocation data)
        {
            try
            {
                var checkProduct = _context.products1.Where(x => x.id == data.id_product && !x.deleted).FirstOrDefault();

                if(checkProduct == null)
                    return await Task.FromResult(PayLoad<bool>.CreatedFail(Status.DATANULL));

                if (!checkLocationExsis(checkProduct.category_map.Value, data.id_Shelf, data.location))
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
                var checkId = _context.products1.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;

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
                var checkdata = _context.products1.Where(x => !x.deleted).ToList();

                var dataMapList = loadDataCategory(checkdata);
                if (!string.IsNullOrEmpty(name))
                    dataMapList = dataMapList.Where(x => x.title.Contains(name) || x.supplierName.Contains(name)).ToList();

                var pageList = new PageList<object>(dataMapList, page - 1, pageSize);
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

        public async Task<PayLoad<object>> FindOneByShelf(int id)
        {
            try
            {
                var checkArea = _context.shelfs.Where(x => x.id == id && !x.deleted).FirstOrDefault();

                if (checkArea == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(findAreaproduct(checkArea)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private productShelf findAreaproduct(Shelf shelf)
        {
            var data = new productShelf();
            data.Id = shelf.id;
            data.quantity = shelf.quantity.Value;
            data.name = shelf.name;
            data.totalLocation = totalQuantityLocation(shelf);
            data.totalLocationEmpty = CheckLocationUsed(shelf);
            data.totalLocatiEmpty = totalQuantityLocation(shelf) - CheckLocationUsed(shelf);
            data.productLocationAreas = productLocationShelf(shelf.id);
            data.productPlans = productLocationShelfPlan(shelf.id);
            data.locationTotal = checkLocation(shelf.id);
            data.warehoursPlans = loadDataWarehoursePlan(shelf.id);
            data.totalLocatiExsis = shelf.quantity.Value - checkLocationAreaExsis(shelf);

            return data;
        }
        private int checkLocationUsed(Shelf shelf, int location)
        {
            var checkData = _context.productlocations.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted && x.isaction).Sum(x => x.quantity);
            return checkData;
        }
        private int CheckLocationUsed(Shelf shelf)
        {
            int sum = 0;

            for (var i = 1; i <= shelf.quantity; i++)
            {
                sum += checkLocationUsed(shelf, i);
            }
            return sum;
        }

        private int checkLocationAreaExsis(Shelf shelf)
        {
            var list = new List<int>();

            var checkLocationTotal = _context.productlocations.Where(x => x.id_shelf == shelf.id && !x.deleted).ToList();
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

            var checkPlan = _context.plans.Where(x => x.shelf == id && x.iswarehourse && !x.deleted).ToList();
            if(checkPlan != null && checkPlan.Any())
            {
                foreach(var item in checkPlan)
                {
                    var DataItem = new WarehoursPlan();
                    var checkShelf = _context.shelfs.Where(x => x.id == item.shelf && !x.deleted).FirstOrDefault();
                    if(checkShelf != null)
                    {
                        DataItem.shelf = checkShelf.name;
                        var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                        if(checkArea != null)
                        {
                            DataItem.area = checkArea.name;
                            var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                            if (checkFloor != null)
                            {
                                DataItem.floor = checkFloor.name;
                                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                                if (checkWarehourse != null)
                                    DataItem.warehours = checkWarehourse.name;
                            }
                        }
                    }

                    list.Add(DataItem);
                }
                
            }

            return list;
        }
        private int totalLocationEmty(Shelf area)
        {
            var checkTotal = _context.productlocations.Where(x => x.id_shelf == area.id && !x.deleted && x.isaction).Sum(x => x.quantity);

            return checkTotal;
        }
        private int? checkQuantityEmty(Shelf area)
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

        private bool checkAreaLocationExsis(Shelf shelf, int location)
        {
            var checkDataProductLocation = _context.productlocations.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted && x.isaction).FirstOrDefault();
            if (checkDataProductLocation != null)
                return false;
            return true;
        }

        private int? quantityArea(Shelf shelf, int location)
        {
            var checkLocation = _context.locationexceptions.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted).FirstOrDefault();
            if (checkLocation != null)
            {
                return checkLocation.max;
            }
            else
            {
                var checkLocationArea = _context.shelfs.Where(x => x.id == shelf.id && !x.deleted).FirstOrDefault();
                if(checkLocationArea != null)
                    return checkLocationArea.max;
            }
            return 0;
        }

        private int totalQuantityLocation(Shelf data)
        {
            var checkLocationExceps = _context.locationexceptions.Where(x => x.id_shelf == data.id && !x.deleted).Count();
            var total = data.quantity - checkLocationExceps;

            var totalNoExCeps = (total * data.max) + totalLocal(data.id);

            return totalNoExCeps.Value;
        }

        private int totalLocal(int id)
        {
            var checkArea = _context.locationexceptions.Where(x => x.id_shelf == id && !x.deleted).Sum(x => x.max);

            return checkArea.Value;
        }
        private Dictionary<int, int> checkLocation(int id)
        {
            var dictionary = new Dictionary<int, int>();
            //var checkPlan = _context.plans.Where(x => x.shelfs == id && !x.iswarehourse).ToList();
            var checkPlan = _context.plans.Where(x => x.shelf == id).ToList();
            if (checkPlan != null && checkPlan.Any())
            {
                foreach(var item in checkPlan)
                {
                    if (!dictionary.ContainsKey(item.localtionnew.Value))
                    {
                        var checkLocation = _context.plans.Where(x => x.shelf == id && x.localtionnew == item.localtionnew && x.status.ToLower() == Status.DONE.ToLower() && x.isconfirmation).Count();
                        dictionary.Add(item.localtionnew.Value, checkLocation);
                    }
                }
            }
            return dictionary;
        }

        private List<productLocationArea> productLocationShelf(int id)
        {
            var list = new List<productLocationArea>();
            var checkData = _context.productlocations.Where(x => x.id_shelf == id && !x.deleted && x.isaction).ToList();

            foreach(var item in checkData)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                if (checkProduct != null)
                {
                    var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.deleted).FirstOrDefault();
                    var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.deleted).FirstOrDefault();
                    var checkAccountCreate = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.deleted).FirstOrDefault();
                    var imageProductData = _context.imageproducts.Where(x => x.productmap == checkProduct.id && !x.deleted).ToList();
                    var checkLocationCode = _context.codelocations.Where(x => x.id_helf == id && x.location == item.location && !x.deleted).FirstOrDefault();
                    var dataItem = new productLocationArea
                    {
                        Id = item.id,
                        Id_product = checkProduct.id,
                        name = checkProduct.title,
                        image = imageProductData[0].link,
                        images = imageProductData.Select(x => x.link),
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
        private List<productLocationArea> productLocationShelfPlan(int id)
        {
            var list = new List<productLocationArea>();

            var checkPlan = _context.plans.Where(x => x.shelf == id && !x.deleted && x.status.ToLower() != Status.DONE.ToLower()).ToList();
            if (checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    var checkLocationProduct = _context.productlocations.Where(x => x.location == item.localtionold && x.id_shelf == item.shelfOld && !x.deleted && x.isaction).ToList();
                    if (checkLocationProduct != null && checkLocationProduct.Count > 0)
                    {
                        foreach (var itemPlan in checkLocationProduct)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == itemPlan.id_product && !x.deleted).FirstOrDefault();
                            if (checkProduct != null)
                            {
                                var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.deleted).FirstOrDefault();
                                var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.deleted).FirstOrDefault();
                                var checkAccountCreate = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.deleted).FirstOrDefault();
                                var imageProductData = _context.imageproducts.Where(x => x.productmap == checkProduct.id && !x.deleted).ToList();
                                var checkLocationCode = _context.codelocations.Where(x => x.id_helf == id && x.location == item.localtionnew && !x.deleted).FirstOrDefault();
                                var dataItem = new productLocationArea
                                {
                                    Id_product = checkProduct.id,
                                    location = item.localtionnew,
                                    Id = itemPlan.id,
                                    image = imageProductData[0].link,
                                    images = imageProductData.Select(x => x.link),
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
        //private List<productLocationArea> productLocationShelfPlan(int id)
        //{
        //    var list = new List<productLocationArea>();

        //    var checkPlan = _context.plans.Where(x => x.shelfs == id && !x.deleted && x.status.ToLower() != status.DONE.ToLower()).ToList();
        //    if (checkPlan.Any())
        //    {
        //        foreach (var item in checkPlan)
        //        {
        //            var checkLocationProduct = _context.productlocations.Where(x => x.location == item.localtionnew && x.id_shelf == item.shelfs && !x.deleted).FirstOrDefault();
        //            var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.deleted).FirstOrDefault();
        //            var imageProductData = _context.imageproducts.Where(x => x.productmap == checkProduct.id && !x.deleted).FirstOrDefault();
        //            var checkLocationCode = _context.codelocations.Where(x => x.id_shelf == id && x.location == item.localtionnew && !x.deleted).FirstOrDefault();
        //            var dataItem = new productLocationArea
        //            {
        //                Id_product = checkProduct.id,
        //                location = item.localtionnew,
        //                Id = checkLocationProduct.id,
        //                image = imageProductData.link,
        //                name = checkProduct.title,
        //                quantity = checkLocationProduct.quantity,
        //                Id_plan = item.id,
        //                code = checkLocationCode == null ? status.CODEFAILD : checkLocationCode.code
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
                var checkIdCategory = _context.products1.Where(x => x.category_map == id && !x.deleted).ToList();

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
                    var checkSupplier = _context.suppliers.Where(x => x.id == item.suppliers && !x.deleted).FirstOrDefault();
                    var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.deleted).FirstOrDefault();
                    var mapData = _mapper.Map<ProductGetAll>(item);
                    mapData.categoryId = checkCategory == null ? null : checkCategory.id;
                    mapData.categoryImage = checkCategory == null ? Status.NOCATEGORY : checkCategory.image;
                    mapData.categoryName = checkCategory == null ? Status.NOCATEGORY : checkCategory.name;
                    mapData.supplierId = checkSupplier == null ? null : checkSupplier.id;
                    mapData.supplierImage = checkSupplier == null ? Status.DATANULL : checkSupplier.image;
                    mapData.supplierName = checkSupplier == null ? Status.DATANULL : checkSupplier.name;
                    mapData.images = ListImage(item.id);
                    mapData.listAreaOfproducts = loadDataAreaAndFloorAndWerahourse(item.id);
                    mapData.historyProductLocations = listDataHistory(item);
                    list.Add(mapData);
                }
            }
            return list;
        }

        private List<historyProductLocation> listDataHistory(product item)
        {
            var list = new List<historyProductLocation>();
            var checkData = _context.producthisstorylocations.Where(x => x.product_id == item.id && !x.deleted).ToList();
            var checkDataOld = _context.producthisstorylocations.Where(x => x.product_old == item.id && !x.deleted).ToList();

            if(checkData != null && checkData.Count > 0)
            {
                foreach (var itemLocation in checkData)
                {
                    list.Add(loadDataLoctionHistoryNew(itemLocation));
                }
                
            }
            if(checkDataOld != null && checkDataOld.Count > 0)
            {
                foreach (var itemLocation in checkDataOld)
                {
                    list.Add(loadDataLoctionHistoryOld(itemLocation));
                }
            }

            return list;
        }

        private historyProductLocation loadDataLoctionHistoryNew(producthisstorylocation item)
        {
            var checkShelf = _context.shelfs.Include(l => l.line_id).Where(x => x.id == item.shelfnew && !x.deleted).FirstOrDefault();
            var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == checkShelf.line_id.id_area && !x.deleted).FirstOrDefault();
            var checkWarehouse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.deleted).FirstOrDefault();
            var checkCode = _context.codelocations.Where(x => x.id_helf == checkShelf.id && x.location == item.locationnew && !x.deleted).FirstOrDefault();
            var dataItem = new historyProductLocation();
            dataItem.type = Status.NEW;
            dataItem.idWarehouse = checkWarehouse == null ? null : checkWarehouse.id;
            dataItem.idFloor = checkArea == null || checkArea.floor_id == null ? null : checkArea.floor_id.id;
            dataItem.idArea = checkArea == null ? null : checkArea.id;
            dataItem.idShelf = checkShelf == null ? null : checkShelf.id;

            dataItem.Warehouse_name = checkWarehouse == null ? Status.DATANULL : checkWarehouse.name;
            dataItem.Warehouse_image = checkWarehouse == null ? Status.DATANULL : checkWarehouse.image;
            dataItem.Floor_name = checkArea == null || checkArea.floor_id == null ? Status.DATANULL : checkArea.floor_id.name;
            dataItem.Floor_image = checkArea == null || checkArea.floor_id == null ? Status.DATANULL : checkArea.floor_id.image;
            dataItem.Area_name = checkArea == null ? Status.DATANULL : checkArea.name;
            dataItem.Area_image = checkArea == null ? Status.DATANULL : checkArea.image;
            dataItem.Shelf_name = checkShelf == null ? Status.DATANULL : checkShelf.name;
            dataItem.Shelf_image = checkShelf == null ? Status.DATANULL : checkShelf.image;
            dataItem.code = checkCode == null ? Status.DATANULL : checkCode.code;
            dataItem.lcoation = item.locationnew;

            return dataItem;

        }

        private historyProductLocation loadDataLoctionHistoryOld(producthisstorylocation item)
        {
            var checkShelf = _context.shelfs.Include(l => l.line_id).Where(x => x.id == item.shelfold && !x.deleted).FirstOrDefault();
            var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == checkShelf.line_id.id_area && !x.deleted).FirstOrDefault();
            var checkWarehouse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.deleted).FirstOrDefault();
            var checkCode = _context.codelocations.Where(x => x.id_helf == item.shelfold && x.location == item.locationold && !x.deleted).FirstOrDefault();

            var dataItem = new historyProductLocation();
            dataItem.type = Status.OLD;
            dataItem.idWarehouse = checkWarehouse == null ? null : checkWarehouse.id;
            dataItem.idFloor = checkArea == null || checkArea.floor_id == null ? null : checkArea.floor_id.id;
            dataItem.idArea = checkArea == null ? null : checkArea.id;
            dataItem.idShelf = checkShelf == null ? null : checkShelf.id;

            dataItem.Warehouse_name = checkWarehouse == null ? Status.DATANULL : checkWarehouse.name;
            dataItem.Warehouse_image = checkWarehouse == null ? Status.DATANULL : checkWarehouse.image;
            dataItem.Floor_name = checkArea == null || checkArea.floor_id == null ? Status.DATANULL : checkArea.floor_id.name;
            dataItem.Floor_image = checkArea == null || checkArea.floor_id == null ? Status.DATANULL : checkArea.floor_id.image;
            dataItem.Area_name = checkArea == null ? Status.DATANULL : checkArea.name;
            dataItem.Area_image = checkArea == null ? Status.DATANULL : checkArea.image;
            dataItem.Shelf_name = checkShelf == null ? Status.DATANULL : checkShelf.name;
            dataItem.Shelf_image = checkShelf == null ? Status.DATANULL : checkShelf.image;
            dataItem.code = checkCode == null ? Status.DATANULL : checkCode.code;
            dataItem.lcoation = item.locationold;

            return dataItem;

        }
        public async Task<PayLoad<ProductGetAll>> FindOneById(int id)
        {
            try {
                var checkId = _context.products1.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<ProductGetAll>.CreatedFail(Status.DATANULL));

                var checkCategory = _context.categories.Where(x => x.id == checkId.category_map && !x.deleted).FirstOrDefault();
                var mapData = _mapper.Map<ProductGetAll>(checkId);
                if(checkCategory != null)
                {
                    mapData.categoryImage = checkCategory.image;
                    mapData.categoryName= checkCategory.name;
                }
                mapData.images = ListImage(checkId.id);
                //mapData.listAreaOfproducts = loadDataAreaAndFloorAndWerahourse(checkId.id);
                mapData.oneDataShelfOfProducts = dataListShelfOrProduct(checkId.id);

                return await Task.FromResult(PayLoad<ProductGetAll>.Successfully(mapData));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductGetAll>.CreatedFail(ex.Message));
            }
        }

        private List<OneDataShelfOfProduct> dataListShelfOrProduct(int id)
        {
            var list = new List<OneDataShelfOfProduct>();
            var checkDataWarehouse = new List<int?>();

            var checkProductLocation = _context.productlocations.Where(x => x.id_product == id && !x.deleted && x.isaction).ToList();
            if(checkProductLocation != null && checkProductLocation.Count > 0)
            {
                foreach(var item in checkProductLocation)
                {
                    var checkShelf = _context.shelfs.Include(a => a.line_id).Where(x => x.id == item.id_shelf && !x.deleted).FirstOrDefault();
                    if(checkShelf != null && checkShelf.line_id != null)
                    {
                        var checkArea = _context.areas.Where(x => x.id == checkShelf.line_id.id_area && !x.deleted).FirstOrDefault();
                        var checkFloor = _context.floors.Include(w => w.warehouse_id).Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                        var checkAccount = _context.accounts.Where(x => x.id == checkFloor.warehouse_id.account_map && !x.deleted).FirstOrDefault();
                        if (checkDataWarehouse.Contains(checkFloor.warehouse))
                        {
                            var updateData = list.FirstOrDefault(x => x.Id == checkFloor.warehouse);
                            var dataUpdate = dataOneProductWarehouse(item, checkShelf, checkFloor.warehouse_id, checkShelf.line_id, checkArea, checkFloor, checkAccount);
                            updateData.listShelfOfproducts.Add(dataUpdate);
                            updateData.quantity += dataUpdate.quantity.Value;
                        }
                        else
                        {
                            var dataListShelfOfproducts = new List<listAreaOfproduct>();
                            dataListShelfOfproducts.Add(dataOneProductWarehouse(item, checkShelf, checkFloor.warehouse_id, checkShelf.line_id, checkArea, checkFloor, checkAccount));

                            var dataItem = new OneDataShelfOfProduct
                            {
                                Id = checkFloor.warehouse.Value,
                                account_image = checkAccount.image,
                                account_name = checkAccount.username,
                                addressWarehouse = checkFloor.warehouse_id.country + ", " + checkFloor.warehouse_id.city + ", " + checkFloor.warehouse_id.district + ", " + checkFloor.warehouse_id.street,
                                warehouse_image = checkFloor.warehouse_id.image,
                                warehouse_name = checkFloor.warehouse_id.name,
                                quantity = item.quantity,
                                listShelfOfproducts = dataListShelfOfproducts
                            };

                            list.Add(dataItem);
                            checkDataWarehouse.Add(checkFloor.warehouse);
                        }
                    }
                }
            }

            return list;
        }

        private listAreaOfproduct dataOneProductWarehouse(productlocation item, Shelf checkShelf, Warehouse checkWarehourse, Line line,  areas checkArea, Floor checkFloor, accounts checkAccount)
        {
            var checkLocationShelfCode = _context.codelocations.Where(x => x.id_helf == checkShelf.id && x.location == item.location && !x.deleted).FirstOrDefault();
            
            var dataItem = new listAreaOfproduct
            {
                account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image,
                account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username,
                addressWarehouse = checkWarehourse.country + ", " + checkWarehourse.city + ", " + checkWarehourse.district + ", " + checkWarehourse.street,
                warehouse_name = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.name,
                warehouse_image = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.image,
                floor_image = checkFloor == null ? Status.NOFLOOR : checkFloor.image,
                floor_name = checkFloor == null ? Status.NOFLOOR : checkFloor.name,
                area_image = checkArea == null ? Status.NOAREA : checkArea.image,
                area_name = checkArea == null ? Status.NOAREA : checkArea.name,
                shelf_image = checkShelf == null ? Status.NOSHELF : checkShelf.image,
                shelf_name = checkShelf == null ? Status.NOSHELF : checkShelf.name,
                quantity = item.quantity,
                Id_productlocation = item.id,
                location = item.location,
                idShelf = checkShelf.id,
                idArea = checkArea.id,
                idFloor = checkFloor.id,
                line_name = line.name,
                idWarehouse = checkWarehourse.id,
                MaxlocationExceps = QuantityAreaMax(checkShelf.id, item.location),
                MaxlocationShelf = checkShelf.max,
                code = checkLocationShelfCode == null ? Status.CODEFAILD : checkLocationShelfCode.code
            };

            return dataItem;
        }
        private List<listAreaOfproduct> loadDataAreaAndFloorAndWerahourse(int id)
        {
            var list = new List<listAreaOfproduct>();
            var checkProductLocation = _context.productlocations.Where(x => x.id_product == id && !x.deleted && x.isaction).ToList();
            var checkProduct = _context.products1.Include(s =>s.supplier_id).Where(x => x.id == id && !x.deleted).FirstOrDefault();
            if (checkProductLocation.Any())
            {
                foreach (var item in checkProductLocation)
                {
                    var checkShelf = _context.shelfs.Where(x => x.id == item.id_shelf && !x.deleted).FirstOrDefault();
                    if(checkShelf != null)
                    {
                        var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                        if(checkArea != null)
                        {
                            var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                            if(checkFloor != null)
                            {
                                var checkLocationShelfCode = _context.codelocations.Where(x => x.id_helf == checkShelf.id && x.location == item.location && !x.deleted).FirstOrDefault();
                                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                                var checkAccount = _context.accounts.Where(x => x.id == checkWarehourse.account_map && !x.deleted).FirstOrDefault();
                                var dataItem = new listAreaOfproduct
                                {
                                    account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image,
                                    account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username,
                                    addressWarehouse = checkWarehourse.country + ", " + checkWarehourse.city + ", " + checkWarehourse.district + ", " + checkWarehourse.street,
                                    warehouse_name = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.name,
                                    warehouse_image = checkWarehourse == null ? Status.NOWAREHOURSE : checkWarehourse.image,
                                    floor_image = checkFloor == null ? Status.NOFLOOR : checkFloor.image,
                                    floor_name = checkFloor == null ? Status.NOFLOOR : checkFloor.name,
                                    area_image = checkArea == null ? Status.NOAREA : checkArea.image,
                                    area_name = checkArea == null ? Status.NOAREA : checkArea.name,
                                    shelf_image = checkShelf == null ? Status.NOSHELF : checkShelf.image,
                                    shelf_name = checkShelf == null ? Status.NOSHELF : checkShelf.name,
                                    quantity = item.quantity,
                                    Id_productlocation = item.id,
                                    location = item.location,
                                    idShelf = checkShelf.id,
                                    idArea = checkArea.id,
                                    idFloor = checkFloor.id,
                                    idWarehouse = checkWarehourse.id,
                                    MaxlocationExceps = QuantityAreaMax(checkShelf.id, item.location),
                                    MaxlocationShelf = checkShelf.max,
                                    code = checkLocationShelfCode == null ? Status.CODEFAILD : checkLocationShelfCode.code
                                };

                                list.Add(dataItem);
                            }
                        }
                    }
                    
                }
            }

            return list;
        }

        private int QuantityAreaMax(int id, int location)
        {
            var checkProductExceps = _context.locationexceptions.Where(x => x.id_shelf == id && !x.deleted && x.location == location).FirstOrDefault();
            if(checkProductExceps != null)
            {
                return checkProductExceps.max.Value;
            }
            else
            {
                var checkArea = _context.shelfs.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkArea != null)
                    return checkArea.max.Value;
            }
            return 0;
        }
        private List<string> ListImage(int id)
        {
            var list = new List<string>();

            var checkProduct = _context.imageproducts.Where(x => x.productmap == id && !x.deleted).ToList();
            if (checkProduct.Any())
            {
                foreach(var item in checkProduct)
                {
                    list.Add(item.link);
                }
            }
            return list;
        }
        public async Task<PayLoad<ProductDTO>> Update(int id, ProductDTO productDTO)
        {
            try
            {
                var checkId = _context.products1.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                var checkName = _context.products1.Where(x => x.title == productDTO.title && x.title != checkId.title && !x.deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == productDTO.suppliers && !x.deleted).FirstOrDefault();
                var checkCategoty = _context.categories.Where(x => x.id == productDTO.category_map && !x.deleted).FirstOrDefault();
                
                if (checkId == null || checkSupplier == null || checkCategoty == null || checkName != null)
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATANULL));
                if (productDTO.images != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.PRODUCT + checkId.id.ToString(), _cloud);
                    var checkImageProduct = _context.imageproducts.Where(x => x.productmap == checkId.id && !x.deleted).ToList();
                    if(checkImageProduct.Any())
                    {
                        _context.imageproducts.RemoveRange(checkImageProduct);
                        _context.SaveChanges();
                    }

                    UploadImage(productDTO.images, checkId);
                }

                var mapDataUpdate = MapperData.GanData(checkId, productDTO);
                mapDataUpdate.suppliers = checkSupplier.id;
                mapDataUpdate.supplier_id = checkSupplier;
                mapDataUpdate.categoryid123 = checkCategoty;
                mapDataUpdate.category_map = checkCategoty.id;
                mapDataUpdate.updatedat = DateTimeOffset.UtcNow;

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
                var checkData = _context.products1.Where(x => x.suppliers == id && !x.deleted).ToList();

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
                var checkLocationProduct = _context.productlocations.Where(x => x.id == id && !x.deleted && x.isaction).FirstOrDefault();
                if (checkLocationProduct == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.deleted).FirstOrDefault();
                var checkTotalQuantity = _context.productlocations.Where(x => x.id_product == checkProduct.id && x.id != checkLocationProduct.id && !x.deleted && x.isaction).Sum(x => x.quantity);
                if (checkProduct.quantity < checkTotalQuantity + productDTO.quantity)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                var checkShelf = _context.shelfs.Where(x => x.id == productDTO.id_shefl && !x.deleted).FirstOrDefault();
                if(checkShelf == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                if (!checkLocationExsisArea(checkShelf, productDTO.location, productDTO.quantity))
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
                var checkId = _context.productlocations.Where(x => x.id == id && !x.deleted && x.isaction).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkProduct = _context.products1.Where(x => x.id == checkId.id_product && !x.deleted).FirstOrDefault();
                if (checkProduct == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkShelf = _context.shelfs.Where(x => x.id == checkId.id_shelf && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.deleted).FirstOrDefault();
                var checkLocationCode = _context.codelocations.Where(x => x.location == checkId.location && x.id_helf == checkShelf.id).FirstOrDefault();
                
                if (checkShelf == null || checkArea == null || checkFloor == null || checkWarehourse == null)
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
                mapData.shelf_image = checkShelf == null ? Status.NOAREA : checkShelf.image;
                mapData.shelf_name = checkShelf == null ? Status.NOAREA : checkShelf.image;
                mapData.codeLocation = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code;
                mapData.TotalLocationEmty = checkQuantityEmty(checkShelf);
                return await Task.FromResult(PayLoad<object>.Successfully(mapData));
                
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllProductInWarehourse(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                //var data = _context.productlocations.Where(x => !x.deleted && x.isaction).ToList();
                //var dataMap = loadDataProductInLocation(data);
                //if (!string.IsNullOrEmpty(name))
                //    dataMap = dataMap.Where(x => x.title.Contains(name)).ToList();

                var data = _context.warehouses.Include(f => f.floors).Where(x => x.id == id && !x.deleted).FirstOrDefault();
                var dataMap = dataOneProductByWarehouse(data);
                Console.WriteLine(data?.id.GetType().Name); // Kiểm tra kiểu dữ liệu của id
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

        private List<productWarehouse> dataOneProductByWarehouse(Warehouse data)
        {
            productWarehousesData = new List<productWarehouse>();

            if (data.floors != null && data.floors.Count > 0)
            {
                foreach(var itemFloor in data.floors)
                {
                    var checkArea = _context.areas.Include(s => s.lines).Where(x => x.floor == itemFloor.id && !x.deleted).ToList();
                    if(checkArea != null && checkArea.Count > 0)
                    {
                        dataProductByWarehouse(checkArea, itemFloor, data);
                    }
                }
            }
            return productWarehousesData;
        }

        private void dataProductByWarehouse(List<areas> checkArea, Floor floor, Warehouse warehouse)
        {
            foreach(var itemArea in checkArea)
            {
                dataProductByShelf(itemArea, floor, warehouse);
            }
            
        }

        private void dataProductByShelf(areas itemArea, Floor floor, Warehouse warehouse)
        {
            foreach (var item in itemArea.lines)
            {
                var checkShelfs = _context.shelfs.Where(x => x.line == item.id && !x.deleted).ToList();
                foreach(var itemShelf in checkShelfs)
                {
                    dataLineProduct(item, itemShelf, warehouse, floor, itemArea);
                }
            }
        }

        private void dataLineProduct(Line line, Shelf item, Warehouse warehouse, Floor floor, areas itemArea)
        {
            var checkProductLocation = _context.productlocations.Where(x => x.id_shelf == item.id && !x.deleted && x.isaction).ToList();
            if (checkProductLocation != null && checkProductLocation.Count > 0)
            {
                foreach (var itemProductLocation in checkProductLocation)
                {
                    var checkProduct = _context.products1.Where(x => x.id == itemProductLocation.id_product && !x.deleted).FirstOrDefault();
                    if (checkProduct != null)
                    {
                        var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.deleted).FirstOrDefault();
                        var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.deleted).FirstOrDefault();
                        var checkCodeLocation = _context.codelocations.Where(x => x.id_helf == item.id && x.location == itemProductLocation.location && !x.deleted).FirstOrDefault();
                        var checkImageProduct = _context.imageproducts.Where(x => x.productmap == checkProduct.id && !x.deleted).ToList();
                        var dataMap = _mapper.Map<productWarehouse>(checkProduct);
                        dataMap.images = checkImageProduct.Select(x => x.link).ToList();
                        dataMap.categoryImage = checkCategory.image;
                        dataMap.categoryName = checkCategory.name;
                        dataMap.codeLocation = checkCodeLocation.code;
                        dataMap.quantityLocaton = itemProductLocation.quantity;
                        dataMap.warehouse_name = warehouse.name;
                        dataMap.warehouse_image = warehouse.image;
                        dataMap.floor_name = floor.name;
                        dataMap.floor_image = floor.image;
                        dataMap.area_image = itemArea.image;
                        dataMap.area_name = itemArea.name;
                        dataMap.shelf_image = item.image;
                        dataMap.shelf_name = item.name;
                        dataMap.line_name = line.name;
                        dataMap.suppliers_image = checkSupplier == null ? Status.DATANULL : checkSupplier.image;
                        dataMap.suppliers_name = checkSupplier == null ? Status.DATANULL : checkSupplier.name;
                        dataMap.location = itemProductLocation.location;

                        productWarehousesData.Add(dataMap);
                    }

                }
            }
        }
        private List<productWarehouse> findOneDataShelf(Shelf item, Warehouse warehouse, Floor floor, areas itemArea)
        {
            var checkProductLocation = _context.productlocations.Where(x => x.id_shelf == item.id && !x.deleted && x.isaction).ToList();
            if (checkProductLocation != null && checkProductLocation.Count > 0)
            {
                foreach (var itemProductLocation in checkProductLocation)
                {
                    var checkProduct = _context.products1.Where(x => x.id == itemProductLocation.id_product && !x.deleted).FirstOrDefault();
                    if (checkProduct != null)
                    {
                        var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.deleted).FirstOrDefault();
                        var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.deleted).FirstOrDefault();
                        var checkCodeLocation = _context.codelocations.Where(x => x.id_helf == item.id && x.location == itemProductLocation.location && !x.deleted).FirstOrDefault();
                        var checkImageProduct = _context.imageproducts.Where(x => x.productmap == checkProduct.id && !x.deleted).ToList();
                        var dataMap = _mapper.Map<productWarehouse>(checkProduct);
                        dataMap.images = checkImageProduct.Select(x => x.link).ToList();
                        dataMap.categoryImage = checkCategory.image;
                        dataMap.categoryName = checkCategory.name;
                        dataMap.codeLocation = checkCodeLocation.code;
                        dataMap.quantityLocaton = itemProductLocation.quantity;
                        dataMap.warehouse_name = warehouse.name;
                        dataMap.warehouse_image = warehouse.image;
                        dataMap.floor_name = floor.name;
                        dataMap.floor_image = floor.image;
                        dataMap.area_image = itemArea.image;
                        dataMap.area_name = itemArea.name;
                        dataMap.shelf_image = item.image;
                        dataMap.shelf_name = item.name;
                        dataMap.suppliers_image = checkSupplier == null ? Status.DATANULL : checkSupplier.image;
                        dataMap.suppliers_name = checkSupplier == null ? Status.DATANULL : checkSupplier.name;
                        dataMap.location = itemProductLocation.location;

                        productWarehousesData.Add(dataMap);
                    }

                }
            }

            return productWarehousesData;
        }
        private List<ProductOneLocation> loadDataProductInLocation(List<productlocation> data)
        {
            var list = new List<ProductOneLocation>();

            if(data != null)
            {
                foreach(var item in data)
                {
                    var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                    if(checkProduct != null)
                    {
                        var mapData = _mapper.Map<ProductOneLocation>(checkProduct);
                        var checkShelf = _context.shelfs.Where(x => x.id == item.id_shelf && !x.deleted).FirstOrDefault();
                        if(checkShelf != null)
                        {
                            var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                            if(checkArea != null)
                            {
                                var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                                if (checkFloor != null)
                                {
                                    var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                                    if (checkWarehourse != null)
                                    {
                                        var checkLocationCode = _context.codelocations.Where(x => x.location == item.location && x.id_helf == checkShelf.id && !x.deleted).FirstOrDefault();
                                        mapData.warehouse_image = checkWarehourse.image;
                                        mapData.warehouse_name = checkWarehourse.name;
                                        mapData.floor_image = checkFloor.image;
                                        mapData.floor_name = checkFloor.name;
                                        mapData.area_name = checkArea.name;
                                        mapData.area_image = checkArea.image;
                                        mapData.shelf_image = checkShelf.image;
                                        mapData.shelf_name = checkShelf.name;
                                        mapData.codeLocation = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code;
                                        mapData.TotalLocationEmty = checkQuantityEmty(checkShelf);
                                    }
                                }
                            }
                            mapData.area_image = checkArea == null ? Status.NOAREA : checkArea.image;
                            mapData.area_name = checkArea == null ? Status.NOAREA : checkArea.name;
                        }

                        mapData.images = ListImage(checkProduct.id);
                        mapData.Id = item.id;
                        mapData.Id_product = checkProduct.id;
                        mapData.shelf_name = checkShelf == null ? Status.NOSHELF : checkShelf.name;
                        mapData.shelf_image = checkShelf == null ? Status.NOSHELF : checkShelf.image;

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
                var checkCode = _context.products1.Where(x => x.code == code && !x.deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkCategory = _context.categories.Where(x => x.id == checkCode.category_map && !x.deleted).FirstOrDefault();
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
                var checkCode = _context.codelocations.Where(x => x.code == code && !x.deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkLocationProduct = _context.productlocations.Where(x => x.id_shelf == checkCode.id_helf && x.location == checkCode.location && !x.deleted && x.isaction).ToList();

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
                var checkLocation = _context.productlocations.Where(x => x.id_shelf == data.id_shelf && x.location == data.location && !x.deleted && x.isaction).ToList();
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
                var checkProductLocation = _context.productlocations.Where(x => x.id == id && !x.deleted && x.isaction).FirstOrDefault();
                if (checkProductLocation == null)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.DATANULL));

                if (checkProductLocation.quantity < productDTO.quantity)
                    return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(Status.FULLQUANTITY));

                checkProductLocation.quantity -= productDTO.quantity;
                //if (checkProductLocation.quantity - productDTO.quantity == 0)
                //    checkProductLocation.deleted = true;
                _context.productlocations.Update(checkProductLocation);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductAddAreas>.Successfully(productDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductAddAreas>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllProductInFloorAndArea(int id_floor, int id_area, int page = 1, int pageSize = 20)
        {
            try
            {
                productWarehousesData = new List<productWarehouse>();
                var checkFloor = _context.floors.Include(w => w.warehouse_id).Where(x => x.id == id_floor && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Include(s => s.lines).Where(x => x.id == id_area && !x.deleted).FirstOrDefault();
                if (checkFloor == null || checkArea == null || checkFloor.warehouse_id == null || checkArea.lines == null || checkArea.lines.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkWarehouse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();

                dataProductByShelf(checkArea, checkFloor, checkWarehouse);

                return await Task.FromResult(PayLoad<object>.Successfully(productWarehousesData));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
        }

        public async Task<PayLoad<object>> FindAllProductInWarehouseAndFloorAndArea(int id_warehouse, int id_floor, int id_area, int page = 1, int pageSize = 20)
        {
            try
            {
                productWarehousesData = new List<productWarehouse>();
                var checkWarehouse = _context.warehouses.Where(x => x.id == id_warehouse && !x.deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == id_floor && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Include(s => s.lines).Where(x => x.id == id_area && !x.deleted).FirstOrDefault();

                if (checkFloor == null || checkArea == null || checkArea.lines == null || checkArea.lines.Count <= 0 || checkWarehouse == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                dataProductByShelf(checkArea, checkFloor, checkWarehouse);

                return await Task.FromResult(PayLoad<object>.Successfully(productWarehousesData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllProductInWarehouseAndFloorAndAreaAndShelf(int id_warehouse, int id_floor, int id_area, int id_shelf, int page = 1, int pageSize = 20)
        {
            try
            {
                productWarehousesData = new List<productWarehouse>();
                var checkWarehouse = _context.warehouses.Where(x => x.id == id_warehouse && !x.deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == id_floor && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Include(s => s.lines).Where(x => x.id == id_area && !x.deleted).FirstOrDefault();
                var checkShelf = _context.shelfs.Where(x => x.id == id_shelf && !x.deleted).FirstOrDefault();

                if (checkFloor == null || checkArea == null || checkShelf == null || checkWarehouse == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(findOneDataShelf(checkShelf, checkWarehouse, checkFloor, checkArea)));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllProductInFloor(int id_floor, int page = 1, int pageSize = 20)
        {
            try
            {
                productWarehousesData = new List<productWarehouse>();
                var checkFloor = _context.floors.Where(x => x.id == id_floor && !x.deleted).FirstOrDefault();
                
                if (checkFloor == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkWarehouse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.floor == checkFloor.id && !x.deleted).ToList();
                dataProductByWarehouse(checkArea, checkFloor, checkWarehouse);

                return await Task.FromResult(PayLoad<object>.Successfully(productWarehousesData));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllProductSearch(int? idWarehouse, int? idFloor, int? idArea, int? idShelf, int? supplier, int? category ,int? pricefrom, int? priceto, string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkdata = _context.products1.Where(x => !x.deleted).ToList();

                var dataMapList = loadDataCategory(checkdata);
                #region Warehouse
                if (category != null && supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null)
                    dataMapList = dataMapList.Where(x => x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.supplierId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.price == priceto).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.supplierId == supplier && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse)).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor)).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea)).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf)).ToList();

                else if (supplier == null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor)).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea)).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf)).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.supplierId == supplier && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse) && x.supplierId == supplier && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.supplierId == supplier && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == supplier && x.price == pricefrom && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse != null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idFloor == idFloor) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == idShelf).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == idShelf && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == idShelf && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idArea == idArea) && x.supplierId == idShelf && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.supplierId == supplier && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse != null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idWarehouse == idWarehouse && p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();
                #endregion
                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x =>x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea)).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.categoryId == category && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf)).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.price == pricefrom && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea != null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idArea == idArea && p.idShelf == idShelf) && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.price == pricefrom && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.supplierId == supplier && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor != null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idFloor == idFloor && p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf == null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.categoryId == category && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea != null && idShelf != null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idArea == idArea && p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.supplierId == supplier).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto == null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.supplierId == supplier && x.price == pricefrom).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category == null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.supplierId == supplier && x.price >= pricefrom && x.price <= priceto && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier != null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.supplierId == supplier && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom == null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.categoryId == category).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto == null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.categoryId == category && x.price == pricefrom).ToList();

                else if (supplier == null && idWarehouse == null && idFloor == null && idArea == null && idShelf != null && pricefrom != null && priceto != null && category != null)
                    dataMapList = dataMapList.Where(x => x.listAreaOfproducts.Any(p => p.idShelf == idShelf) && x.categoryId == category && x.price >= pricefrom && x.price <= priceto).ToList();
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    dataMapList
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}