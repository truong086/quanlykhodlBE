using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class AreaService : IAreaService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        private KiemTraBase64 _kiemtrabase64;
        public AreaService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService, KiemTraBase64 kiemtrabase64)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
            _kiemtrabase64 = kiemtrabase64;
        }
        public async Task<PayLoad<AreaDTO>> Add(AreaDTO areaDTO)
        {
            try
            {
                var user = _userService.name();
                var checkName = _context.areas.Where(x => x.name == areaDTO.name && !x.Deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                var checkFloor = _context.floors.Where(x => x.id == areaDTO.floor && !x.Deleted).FirstOrDefault();
                if (checkFloor == null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if (!checkFullQuantity(checkFloor, checkFloor.id))
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();

                var mapData = _mapper.Map<Area>(areaDTO);
                mapData.code = RanDomCode.geneAction(8);
                mapData.account = checkAccount.id;
                mapData.account_id = checkAccount;
                mapData.floor = checkFloor.id;
                mapData.floor_id = checkFloor;

                _context.areas.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.areas.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (areaDTO.image != null)
                {
                    if (!_kiemtrabase64.kiemtra(areaDTO.image))
                    {
                        uploadCloud.CloudInaryAccount(areaDTO.image, TokenViewModel.AREA + dataNew.id.ToString(), _cloud);
                    }
                    else
                    {
                        var chuyenDoi = chuyenDoiIFromFileProduct(areaDTO.image, dataNew.id);
                        uploadCloud.CloudInaryIFromAccount(chuyenDoi, TokenViewModel.AREA + dataNew.id.ToString(), _cloud);
                    }

                    dataNew.image = uploadCloud.Link;
                    dataNew.publicid = uploadCloud.publicId;

                    _context.areas.Update(dataNew);
                    _context.SaveChanges();
                }

                if(areaDTO.locationExceptionsDTOs != null)
                {
                    updateLocationExcep(areaDTO.locationExceptionsDTOs, dataNew);
                }

                addCodeLocation(dataNew, dataNew.quantity.Value);
                return await Task.FromResult(PayLoad<AreaDTO>.Successfully(areaDTO));


            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(ex.Message));
            }
        }

        private IFormFile chuyenDoiIFromFileProduct(string data, int id)
        {
            var chuyenDoiStringBase64 = new ChuyenFile();
            var fileName = "Area" + id;
            return chuyenDoiStringBase64.chuyendoi(data, fileName);
        }

        private void addCodeLocation(Area area, int location)
        {
            for(var i = 1; i <= location; i++)
            {
                var dataItem = new Codelocation
                {
                    area = area,
                    code = RanDomCode.geneAction(8) + area.id.ToString(),
                    id_area = area.id,
                    location = i
                };

                _context.codelocations.Add(dataItem);
                _context.SaveChanges();
            }
        }
        private void updateLocationExcep(List<locationExceptionsDTO> data, Area area)
        {
            var list = new List<int>();
            foreach (var item in data)
            {
                if (!list.Contains(item.location))
                {
                    if (item.location < area.quantity)
                    {
                        var localExceps = new LocationException
                        {
                            area = area,
                            id_area = area.id,
                            location = item.location,
                            max = item.quantity
                        };

                        _context.locationExceptions.Add(localExceps);
                        _context.SaveChanges();

                        list.Add(item.location);
                    }
                }
            }
        }
        private bool checkFullQuantity(Floor floor, int id)
        {
            var checkAreaQuantity = _context.areas.Where(x => x.floor == id && !x.Deleted).Count();
            if (floor.quantityarea < checkAreaQuantity)
                return false;

            return true;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkData = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                checkData.Deleted = true;

                _context.areas.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FinAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.areas.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) && !x.Deleted).ToList();

                var pageList = new PageList<object>(LoadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<AreaGetAll> LoadData(List<Area> data)
        {
            var list = new List<AreaGetAll>();

            foreach (var item in data)
            {
                var checkAccount = _context.accounts.Where(x => x.id == item.account && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == item.floor && !x.Deleted).FirstOrDefault();
                var checkProductLocation = _context.productlocations.Where(x => x.id_area == item.id && !x.Deleted && x.isAction).Count();
                var mapData = _mapper.Map<AreaGetAll>(item);
                mapData.Id = item.id;
                mapData.floor_name = checkFloor.name;
                mapData.floor_image = checkFloor.image;
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;
                mapData.quantityEmtity = item.quantity - checkProductLocation;
                mapData.productArea = findAreaproduct(item);

                list.Add(mapData);

            }
            return list;
        }
        private productArea findAreaproduct(Area area)
        {
            var data = new productArea();
            data.Id = area.id;
            data.quantity = area.quantity.Value;
            data.totalLocation = totalQuantityLocation(area);
            data.totalLocationEmpty = checkQuantityEmty(area).Value;
            data.totalLocatiEmpty = totalQuantityLocation(area) - checkQuantityEmty(area).Value;
            data.productLocationAreas = productLocationAreas(area.id);
            data.productPlans = productLocationAreasPlan(area.id);
            data.locationTotal = checkLocation(area.id);
            data.warehoursPlans = loadDataWarehoursePlan(area.id);

            return data;
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
                if (checkLocationArea != null)
                    return checkLocationArea.max;
            }
            return 0;
        }

        private bool checkAreaLocationExsis(Area area, int location)
        {
            var checkDataProductLocation = _context.productlocations.Where(x => x.id_area == area.id && x.location == location && !x.Deleted && x.isAction).FirstOrDefault();
            if (checkDataProductLocation != null)
                return false;
            return true;
        }
        private List<WarehoursPlan> loadDataWarehoursePlan(int id)
        {
            var list = new List<WarehoursPlan>();

            var checkPlan = _context.plans.Where(x => x.area == id && x.isWarehourse && !x.Deleted).ToList();
            if (checkPlan != null && checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    var DataItem = new WarehoursPlan();
                    var checkArea = _context.areas.Where(x => x.id == item.area && !x.Deleted).FirstOrDefault();
                    if (checkArea != null)
                    {
                        DataItem.area = checkArea.name;
                        var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.Deleted).FirstOrDefault();
                        if (checkFloor != null)
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
        private Dictionary<int, int> checkLocation(int id)
        {
            var dictionary = new Dictionary<int, int>();
            var checkPlan = _context.plans.Where(x => x.area == id).ToList();
            if (checkPlan != null && checkPlan.Any())
            {
                foreach (var item in checkPlan)
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
        private List<productLocationArea> productLocationAreasPlan(int id)
        {
            var list = new List<productLocationArea>();

            var checkPlan = _context.plans.Where(x => x.area == id && !x.Deleted && x.status.ToLower() != Status.DONE.ToLower()).ToList();
            if (checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    var checkLocationProduct = _context.productlocations.Where(x => x.location == item.localtionOld && x.id_area == item.areaOld && !x.Deleted && x.isAction).FirstOrDefault();
                    if(checkLocationProduct != null)
                    {
                        var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.Deleted).FirstOrDefault();
                        if(checkProduct != null)
                        {
                            var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.Deleted).FirstOrDefault();
                            var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.Deleted).FirstOrDefault();
                            var checkAccountCreate = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();
                            var imageProductData = _context.imageProducts.Where(x => x.productMap == checkProduct.id && !x.Deleted).FirstOrDefault();
                            var checkLocationCode = _context.codelocations.Where(x => x.id_area == id && x.location == item.localtionNew && !x.Deleted).FirstOrDefault();
                            var dataItem = new productLocationArea
                            {
                                Id_product = checkProduct.id,
                                location = item.localtionNew,
                                Id = checkLocationProduct.id,
                                image = imageProductData.Link,
                                name = checkProduct.title,
                                quantity = checkLocationProduct.quantity,
                                Id_plan = item.id,
                                supplier = checkSupplier.name,
                                account_image = checkAccountCreate.image,
                                account_name = checkAccountCreate.username,
                                category = checkCategory.name,
                                code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code,
                                Inventory = checkProduct.quantity,
                                price = checkProduct.price
                            };

                            list.Add(dataItem);
                        }
                    }
                }
            }

            return list;
        }
        private List<productLocationArea> productLocationAreas(int id)
        {
            var list = new List<productLocationArea>();
            var checkData = _context.productlocations.Where(x => x.id_area == id && !x.Deleted && x.isAction).ToList();

            foreach (var item in checkData)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                if(checkProduct != null)
                {
                    var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.Deleted).FirstOrDefault();
                    var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.Deleted).FirstOrDefault();
                    var checkAccountCreate = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();
                    var imageProductData = _context.imageProducts.Where(x => x.productMap == checkProduct.id && !x.Deleted).FirstOrDefault();
                    var checkLocationCode = _context.codelocations.Where(x => x.id_area == id && x.location == item.location && !x.Deleted).FirstOrDefault();
                    var dataItem = new productLocationArea
                    {
                        Id = item.id,
                        Id_product = checkProduct.id,
                        name = checkProduct.title,
                        image = imageProductData.Link,
                        location = item.location,
                        quantity = item.quantity,
                        supplier = checkSupplier.name,
                        account_image = checkAccountCreate.image,
                        account_name = checkAccountCreate.username,
                        category = checkCategory.name,
                        code = checkLocationCode == null ? Status.CODEFAILD : checkLocationCode.code,
                        Inventory = checkProduct.quantity,
                        price = checkProduct.price
                    };
                    list.Add(dataItem);
                }
            }
            return list;
        }
        private int totalQuantityLocation(Area data)
        {
            var checkLocationExceps = _context.locationExceptions.Where(x => x.id_area == data.id && !x.Deleted).Count();
            var total = data.quantity - checkLocationExceps;

            var totalNoExCeps = (total * data.max) + totalLocal(data.id);

            return totalNoExCeps.Value;
        }
        private int? checkQuantityEmty(Area area)
        {
            int? sum = 0;

            for (var i = 1; i <= area.quantity; i++)
            {
                if (checkAreaLocationExsis(area, i))
                {
                    sum += quantityArea(area, i);
                }
            }
            return sum;
        }
        private int totalLocal(int id)
        {
            var checkArea = _context.locationExceptions.Where(x => x.id_area == id && !x.Deleted).Sum(x => x.max);

            return checkArea.Value;
        }

        public async Task<PayLoad<object>> FindOneFloor(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.areas.Where(x => x.floor == id && !x.Deleted).ToList();

                var pageList = new PageList<object>(LoadData(data), page - 1, pageSize);

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

        public async Task<PayLoad<AreaGetAll>> FindOneId(int id)
        {
            try
            {
                var checkData = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<AreaGetAll>.CreatedFail(Status.DATANULL));
                var checkAccount = _context.accounts.Where(x => x.id == checkData.account && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == checkData.floor && !x.Deleted).FirstOrDefault();

                var mapData = _mapper.Map<AreaGetAll>(checkData);
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;
                mapData.floor_image = checkFloor.image;
                mapData.floor_name = checkFloor.name;

                return await Task.FromResult(PayLoad<AreaGetAll>.Successfully(mapData));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaGetAll>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<AreaDTO>> Update(int id, AreaDTO areaDTO)
        {
            try
            {
                var user = _userService.name();
                var checkId = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == areaDTO.floor && !x.Deleted).FirstOrDefault();
                var checkName = _context.areas.Where(x => x.name == areaDTO.name && !x.Deleted).FirstOrDefault();

                if (checkId == null || checkAccount == null || checkFloor == null || checkName != null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if (!checkFullQuantity(checkFloor, checkFloor.id))
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if (!checkQuantityproductInArea(checkId.id, areaDTO.quantity))
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if(areaDTO.image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.AREA + checkId.id.ToString(), _cloud);

                    if (!_kiemtrabase64.kiemtra(areaDTO.image))
                    {
                        uploadCloud.CloudInaryAccount(areaDTO.image, TokenViewModel.AREA + checkId.id.ToString(), _cloud);
                    }
                    else
                    {
                        var chuyenDoi = chuyenDoiIFromFileProduct(areaDTO.image, checkId.id);
                        uploadCloud.CloudInaryIFromAccount(chuyenDoi, TokenViewModel.AREA + checkId.id.ToString(), _cloud);
                    }
                    checkId.image = uploadCloud.Link;
                    checkId.publicid = uploadCloud.publicId;
                    
                }

                if(areaDTO.locationExceptionsDTOs != null)
                {
                    var checkLocationExcep = _context.locationExceptions.Where(x => x.id_area == checkId.id && !x.Deleted).ToList();
                    if(checkLocationExcep.Count > 0 && checkLocationExcep != null)
                    {
                        _context.locationExceptions.RemoveRange(checkLocationExcep);
                        _context.SaveChanges();
                    }

                    updateLocationExcep(areaDTO.locationExceptionsDTOs, checkId);

                }

                if(areaDTO.quantity != null && areaDTO.quantity != 0)
                {
                    var checkTotal = _context.codelocations.Where(x => x.id_area == checkId.id && !x.Deleted).Count();
                    updateLocationCode(checkId, checkTotal, areaDTO.quantity.Value);
                }

                checkId.quantity = areaDTO.quantity;
                checkId.Status = areaDTO.Status;
                checkId.floor = checkFloor.id;
                checkId.floor_id = checkFloor;
                checkId.name = areaDTO.name;

                _context.areas.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<AreaDTO>.Successfully(areaDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(ex.Message));
            }
        }

        private void updateLocationCode(Area area, int totalLocation, int quantity)
        {
            for(var i = 1; i <= quantity; i++)
            {
                totalLocation++;
                var dataItem = new Codelocation
                {
                    area = area,
                    id_area = area.id,
                    location = totalLocation,
                    code = RanDomCode.geneAction(8) + area.id.ToString()
                };

                _context.codelocations.Add(dataItem); 
                _context.SaveChanges();
            }
        }

        private bool checkQuantityproductInArea(int id, int? quantity)
        {
            var checkProducArea = _context.productlocations.Where(x => x.id_area ==  id && !x.Deleted && x.isAction).ToList();
            if(checkProducArea.Any())
            {
                foreach (var item in checkProducArea)
                {
                    var checkLocation = _context.productlocations.Where(x => x.id_product == item.id && !x.Deleted && x.isAction).OrderByDescending(x => x.location).FirstOrDefault();
                    if (quantity < checkLocation.location)
                        return false;
                }
            }

            return true;
        }
    }
}
