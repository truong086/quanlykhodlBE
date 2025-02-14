using AutoMapper;
using CloudinaryDotNet.Core;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.Linq;

namespace quanlykhodl.Service
{
    public class ShelfService : IShelfService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        private KiemTraBase64 _kiemtrabase64;
        public ShelfService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService, KiemTraBase64 kiemtrabase64)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
            _kiemtrabase64 = kiemtrabase64;
        }
        public async Task<PayLoad<ShelfDTO>> Add(ShelfDTO areaDTO)
        {
            try
            {
                var user = _userService.name();
                var checkName = _context.shelfs.Where(x => x.name == areaDTO.name && !x.deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.DATANULL));

                var checkLine = _context.linespage.Where(x => x.id == areaDTO.area && !x.deleted).FirstOrDefault();
                if (checkLine == null)
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.DATANULL));

                var checkLineArea = _context.linespage.Include(x => x.areasids).Where(x => x.id_area == checkLine.id_area && !x.deleted).ToList();
                var indexData = checkLineArea.FindIndex(x => x.id == checkLine.id && !x.deleted);

                if (!checkFullQuantity(checkLine, checkLine.id))
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();

                var mapData = _mapper.Map<Shelf>(areaDTO);
                mapData.account = checkAccount.id;
                mapData.account_id = checkAccount;
                mapData.line = checkLine.id;
                mapData.line_id = checkLine;

                _context.shelfs.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.shelfs.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                var codeIndexLine = indexData >= 1 && indexData <= 9 ? "0" + indexData.ToString() : indexData.ToString();

                if (areaDTO.image != null)
                {
                    if (!_kiemtrabase64.kiemtra(areaDTO.image))
                    {
                        uploadCloud.CloudInaryAccount(areaDTO.image, TokenViewModel.SHELFS + dataNew.id.ToString(), _cloud);
                    }
                    else
                    {
                        var chuyenDoi = chuyenDoiIFromFileProduct(areaDTO.image, dataNew.id);
                        uploadCloud.CloudInaryIFromAccount(chuyenDoi, TokenViewModel.SHELFS + dataNew.id.ToString(), _cloud);
                    }

                    dataNew.image = uploadCloud.Link;
                    dataNew.publicid = uploadCloud.publicId;
                }

                if(areaDTO.locationExceptionsDTOs != null)
                {
                    updateLocationExcep(areaDTO.locationExceptionsDTOs, dataNew);
                }

                //dataNew.code = RanDomCode.geneAction(8) + dataNew.id.ToString();
                
                dataNew.code = checkLine.areasids.name + codeIndexLine + dataNew.id;

                addCodeLocation(dataNew, dataNew.quantity.Value, dataNew.code);
                _context.shelfs.Update(dataNew);
                _context.SaveChanges();
                return await Task.FromResult(PayLoad<ShelfDTO>.Successfully(areaDTO));


            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(ex.Message));
            }
        }

        private IFormFile chuyenDoiIFromFileProduct(string data, int id)
        {
            var chuyenDoiStringBase64 = new ChuyenFile();
            var fileName = "Shelf" + id;
            return chuyenDoiStringBase64.chuyendoi(data, fileName);
        }

        private void addCodeLocation(Shelf shelf, int location, string code)
        {
            for(var i = 1; i <= location; i++)
            {
                var dataItem = new Codelocation
                {
                    shelf = shelf,
                    code = code + i,
                    id_helf = shelf.id,
                    location = i
                };

                _context.codelocations.Add(dataItem);
                _context.SaveChanges();
            }
        }
        private void updateLocationExcep(List<locationExceptionsDTO> data, Shelf shelf)
        {
            var list = new List<int?>();
            foreach (var item in data)
            {
                if (!list.Contains(item.location))
                {
                    if (item.location < shelf.quantity
                        && item.location > 0 
                        && item.quantity > 0
                        && item.location != null
                        && item.quantity != null)
                    {
                        var localExceps = new LocationException
                        {
                            shelf = shelf,
                            id_shelf = shelf.id,
                            location = item.location,
                            max = item.quantity
                        };

                        _context.locationexceptions.Add(localExceps);
                        _context.SaveChanges();

                        list.Add(item.location);
                    }
                }
            }
        }
        private bool checkFullQuantity(Line line, int id)
        {
            var checkAreaQuantity = _context.shelfs.Where(x => x.line == id && !x.deleted).Count();
            if (line.quantityshelf < checkAreaQuantity)
                return false;

            return true;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkData = _context.shelfs.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                checkData.deleted = true;

                _context.shelfs.Update(checkData);
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
                var data = _context.shelfs.Where(x => !x.deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) && !x.deleted).ToList();

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

        private List<ShelfGetAll> LoadData(List<Shelf> data)
        {
            var list = new List<ShelfGetAll>();

            foreach (var item in data)
            {
                
                list.Add(dataFindOneShelf(item));

            }
            return list;
        }

        private ShelfGetAll dataFindOneShelf(Shelf item)
        {
            var checkAccount = _context.accounts.Where(x => x.id == item.account && !x.deleted).FirstOrDefault();
            var checkLine = _context.linespage.Where(x => x.id == item.line && !x.deleted).FirstOrDefault();
            var checkArea = _context.areas.Where(x => x.id == checkLine.id_area && !x.deleted).FirstOrDefault();
            var checkProductLocation = _context.productlocations.Where(x => x.id_shelf == item.id && !x.deleted && x.isaction).Count();

            var mapData = _mapper.Map<ShelfGetAll>(item);
            mapData.Id = item.id;
            mapData.Area_name = checkArea.name;
            mapData.Id_Area = checkArea.id;
            mapData.Area_image = checkArea.image;
            mapData.linenames = checkLine.name;
            mapData.lineidsdata = checkLine.id;
            mapData.account_name = checkAccount.username;
            mapData.account_image = checkAccount.image;
            mapData.quantityEmtity = item.quantity - checkProductLocation;
            mapData.totalLocationExsis = item.quantity - checkLocationAreaExsis(item);
            mapData.productShefl = findAreaproduct(item);
            mapData.totalQuantityUseds = totalQuantityUsedsData(item);
            mapData.quantityExceptions = dataLocationException(item);
            mapData.productInPlans = dataProductInPlan(item);

            return mapData;
        }
        private List<productInPlan> dataProductInPlan(Shelf item)
        {
            var list = new List<productInPlan>();

            var checkPlan = _context.plans.Where(x => (x.shelf == item.id || x.shelfOld == item.id) && x.status.ToLower() != Status.DONE.ToLower() && !x.deleted).ToList();
            foreach(var itemPlan in checkPlan)
            {
                var dataItem = new productInPlan
                {
                    shelfsNew = itemPlan.shelf,
                    shelfsOld = itemPlan.shelfOld,
                    locationNew = itemPlan.localtionnew,
                    locationOld = itemPlan.localtionold,
                    title = itemPlan.title
                };

                list.Add(dataItem);
            }

            return list;
        }
        private List<totalQuantityUsed> totalQuantityUsedsData(Shelf shelf)
        {
            var list = new List<totalQuantityUsed>();

            for(var i = 1; i <= shelf.quantity; i++)
            {
                var checkLocation = _context.productlocations.Where(x => x.id_shelf == shelf.id && x.location == i && !x.deleted && x.isaction).ToList();
                if(checkLocation.Count > 0)
                {
                    var dataItem = new totalQuantityUsed();
                    dataItem.location = i;
                    var checkLocationException = _context.locationexceptions.Where(x => x.id_shelf == shelf.id && x.location == i && !x.deleted).FirstOrDefault();
                    if(checkLocationException != null)
                    {
                        dataItem.quantity = checkLocationException.max.Value;
                        dataItem.quantityUsed = checkLocationException.max.Value - checkLocation.Sum(x => x.quantity);
                    }
                    else
                    {
                        dataItem.quantity = shelf.max.Value;
                        dataItem.quantityUsed = shelf.max.Value - checkLocation.Sum(x => x.quantity);
                    }

                    list.Add(dataItem);
                }
            }

            return list;
        }
        private int checkLocationAreaExsis(Shelf area)
        {
            var list = new List<int>();

            var checkLocationTotal = _context.productlocations.Where(x => x.id_shelf == area.id && !x.deleted).ToList();
            if(checkLocationTotal.Count > 0)
            {
                foreach(var item in checkLocationTotal)
                {
                    if (!list.Contains(item.location))
                        list.Add(item.location);
                }
                
            }
            return list.Count();
        }
        private productShelf findAreaproduct(Shelf area)
        {
            var data = new productShelf();
            data.Id = area.id;
            data.quantity = area.quantity.Value;
            data.totalLocation = totalQuantityLocation(area);
            data.totalLocationEmpty = CheckLocationUsed(area);
            data.totalLocatiEmpty = totalQuantityLocation(area) - CheckLocationUsed(area);
            data.productLocationAreas = productLocationAreas(area.id);
            data.productPlans = productLocationAreasPlan(area.id);
            data.locationTotal = checkLocation(area.id);
            data.warehoursPlans = loadDataWarehoursePlan(area.id);

            return data;
        }
        private int? quantityArea(Shelf area, int location)
        {
            var checkLocation = _context.locationexceptions.Where(x => x.id_shelf == area.id && x.location == location && !x.deleted).FirstOrDefault();
            if (checkLocation != null)
            {
                return checkLocation.max;
            }
            else
            {
               return area.max;
            }
        }

        private bool checkAreaLocationExsis(Shelf area, int location)
        {
            var checkDataProductLocation = _context.productlocations.Where(x => x.id_shelf == area.id && x.location == location && !x.deleted && x.isaction).FirstOrDefault();
            if (checkDataProductLocation != null)
                return false;
            return true;
        }
        private int? checkQuantityEmty(Shelf area)
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
        private List<WarehoursPlan> loadDataWarehoursePlan(int id)
        {
            var list = new List<WarehoursPlan>();

            var checkPlan = _context.plans.Where(x => x.shelf == id && x.iswarehourse && !x.deleted).ToList();
            if (checkPlan != null && checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    var DataItem = new WarehoursPlan();
                    var checkShelf = _context.shelfs.Where(x => x.id == item.shelf && !x.deleted).FirstOrDefault();
                    if (checkShelf != null)
                    {
                        DataItem.shelf = checkShelf.name;
                        var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                        if (checkArea != null)
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
        private Dictionary<int, int> checkLocation(int id)
        {
            var dictionary = new Dictionary<int, int>();
            var checkPlan = _context.plans.Where(x => x.shelf == id && x.isconfirmation).ToList();
            if (checkPlan != null && checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    if (!dictionary.ContainsKey(item.localtionnew.Value))
                    {
                        var checkLocation = _context.plans.Where(x => x.shelf == id && x.localtionnew == item.localtionnew).Count();
                        dictionary.Add(item.localtionnew.Value, checkLocation);
                    }
                }
            }
            return dictionary;
        }
        private List<productLocationArea> productLocationAreasPlan(int id)
        {
            var list = new List<productLocationArea>();

            var checkPlan = _context.plans.Where(x => x.shelf == id && !x.deleted && x.status.ToLower() != Status.DONE.ToLower()).ToList();
            if (checkPlan.Any())
            {
                foreach (var item in checkPlan)
                {
                    var checkLocationProduct = _context.productlocations.Where(x => x.location == item.localtionold && x.id_shelf == item.shelfOld && !x.deleted && x.isaction).ToList();
                    if(checkLocationProduct != null && checkLocationProduct.Count > 0)
                    {
                        foreach(var itemPlan in checkLocationProduct)
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
        private List<productLocationArea> productLocationAreas(int id)
        {
            var list = new List<productLocationArea>();
            var checkData = _context.productlocations.Where(x => x.id_shelf == id && !x.deleted && x.isaction).ToList();

            foreach (var item in checkData)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                if(checkProduct != null)
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
        private int totalQuantityLocation(Shelf data)
        {
            var checkLocationExceps = _context.locationexceptions.Where(x => x.id_shelf == data.id && !x.deleted).Count();
            var total = data.quantity - checkLocationExceps;

            var totalNoExCeps = (total * data.max) + totalLocal(data.id);

            return totalNoExCeps.Value;
        }
        
        private int totalLocal(int id)
        {
            var checkShelf = _context.locationexceptions.Where(x => x.id_shelf == id && !x.deleted).Sum(x => x.max);

            return checkShelf.Value;
        }

        public async Task<PayLoad<object>> FindOneArea(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.shelfs.Where(x => x.line == id && !x.deleted).ToList();

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

        public async Task<PayLoad<ShelfGetAll>> FindOneId(int id)
        {
            try
            {
                var checkData = _context.shelfs.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<ShelfGetAll>.CreatedFail(Status.DATANULL));
                var checkAccount = _context.accounts.Where(x => x.id == checkData.account && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.id == checkData.line && !x.deleted).FirstOrDefault();

                var mapData = _mapper.Map<ShelfGetAll>(checkData);
                mapData.account_name = checkAccount == null ? Status.ACCOUNTFAILD : checkAccount.username;
                mapData.account_image = checkAccount == null ? Status.ACCOUNTFAILD : checkAccount.image;
                mapData.Area_image = checkArea == null ? Status.NOAREA : checkArea.image;
                mapData.Area_name = checkArea == null ? Status.NOAREA : checkArea.name;
                mapData.Id_Area = checkArea == null ? null : checkArea.id;
                mapData.imageShelf = checkData.image;
                mapData.quantityExceptions = dataLocationException(checkData);

                return await Task.FromResult(PayLoad<ShelfGetAll>.Successfully(mapData));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ShelfGetAll>.CreatedFail(ex.Message));
            }
        }

        private List<quantityException> dataLocationException(Shelf area)
        {
            var list = new List<quantityException>();

            var checkData = _context.locationexceptions.Where(x => x.id_shelf == area.id && !x.deleted).ToList();
            if(checkData.Count > 0)
            {
                foreach(var item in checkData)
                {
                    var dataItem = new quantityException { 
                        location = item.location,
                        quantity = item.max
                    };

                    list.Add(dataItem);
                }
            }

            return list;
        }

        public async Task<PayLoad<ShelfDTO>> Update(int id, ShelfDTO shelf)
        {
            try
            {
                var user = _userService.name();
                var checkId = _context.shelfs.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                var checkLine = _context.linespage.Where(x => x.id == shelf.area && !x.deleted).FirstOrDefault();
                var checkName = _context.shelfs.Where(x => x.name == shelf.name && x.name != checkId.name && !x.deleted).FirstOrDefault();

                if (checkId == null || checkAccount == null || checkLine == null || checkName != null)
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.DATANULL));

                if (!checkFullQuantity(checkLine, checkLine.id))
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.DATANULL));

                if (!checkQuantityproductInArea(checkId.id, shelf.quantity))
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.DATANULL));

                if(!checkQuantitylocationMax(checkId.id, shelf.max.Value))
                    return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.FULLQUANTITY));

                if (shelf.locationExceptionsDTOs != null && shelf.locationExceptionsDTOs.Count() > 0)
                {
                    if (!checkLocationException(shelf.locationExceptionsDTOs))
                        return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(Status.ERRORLOCATION));

                    var checkLocationExcep = _context.locationexceptions.Where(x => x.id_shelf == checkId.id && !x.deleted).ToList();
                    if (checkLocationExcep.Count > 0 && checkLocationExcep != null)
                    {
                        _context.locationexceptions.RemoveRange(checkLocationExcep);
                        _context.SaveChanges();
                    }

                    updateLocationExcep(shelf.locationExceptionsDTOs, checkId);

                }

                if (shelf.image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.SHELFS + checkId.id.ToString(), _cloud);

                    if (!_kiemtrabase64.kiemtra(shelf.image))
                    {
                        uploadCloud.CloudInaryAccount(shelf.image, TokenViewModel.SHELFS + checkId.id.ToString(), _cloud);
                    }
                    else
                    {
                        var chuyenDoi = chuyenDoiIFromFileProduct(shelf.image, checkId.id);
                        uploadCloud.CloudInaryIFromAccount(chuyenDoi, TokenViewModel.SHELFS + checkId.id.ToString(), _cloud);
                    }
                    checkId.image = uploadCloud.Link;
                    checkId.publicid = uploadCloud.publicId;
                }

                if (shelf.quantity != null && shelf.quantity != 0)
                {
                    var checkTotal = _context.codelocations.Where(x => x.id_helf == checkId.id && !x.deleted).Count();
                    updateLocationCode(checkId, checkTotal, shelf.quantity.Value);
                }

                checkId.max = shelf.max;
                checkId.quantity += shelf.quantity;
                checkId.status = shelf.Status;
                checkId.line = checkLine.id;
                checkId.line_id = checkLine;
                checkId.name = shelf.name;

                _context.shelfs.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ShelfDTO>.Successfully(shelf));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ShelfDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkQuantitylocationMax(int shelf, int max)
        {
            var checkListProductLocation = _context.productlocations.Where(x => x.id_shelf == shelf && !x.deleted && x.isaction).ToList();
            var checkLocation = new List<int>();
            if(checkListProductLocation != null && checkListProductLocation.Any())
            {
                foreach(var item in checkListProductLocation)
                {
                    var checkLocationException = _context.locationexceptions.Where(x => x.id_shelf == shelf && x.location == item.location && !x.deleted).FirstOrDefault();
                    if (!checkLocation.Contains(item.location) && checkLocationException == null)
                    {
                        var checkQuantityLocation = _context.productlocations.Where(x => x.id_shelf == shelf && x.location == item.location && !x.deleted && x.isaction).Sum(x => x.quantity);
                        if(checkQuantityLocation > max)
                            return false;

                        checkLocation.Add(item.location);
                    }
                }
            }
            return true;
        }
        private bool checkLocationException(List<locationExceptionsDTO> data)
        {
            foreach(var item in data)
            {
                if (item.location == 0
                 || item.location == null
                 || item.quantity == 0
                 || item.quantity == null)
                    return false;
            }
            return true;
        }

        private void updateLocationCode(Shelf area, int totalLocation, int quantity)
        {
            for(var i = 1; i <= quantity; i++)
            {
                totalLocation++;
                var dataItem = new Codelocation
                {
                    shelf = area,
                    id_helf = area.id,
                    location = totalLocation,
                    code = RanDomCode.geneAction(8) + area.id.ToString() + totalLocation.ToString()
                };

                _context.codelocations.Add(dataItem); 
                _context.SaveChanges();
            }
        }

        private bool checkQuantityproductInArea(int id, int? quantity)
        {
            var checkProducArea = _context.productlocations.Where(x => x.id_shelf ==  id && !x.deleted && x.isaction).ToList();
            if(checkProducArea.Any())
            {
                foreach (var item in checkProducArea)
                {
                    var checkLocation = _context.productlocations.Where(x => x.id_product == item.id && !x.deleted && x.isaction).OrderByDescending(x => x.location).FirstOrDefault();
                    if (quantity < checkLocation.location)
                        return false;
                }
            }

            return true;
        }

        public async Task<PayLoad<object>> FindByDataAreaLineByFloor(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.floor == id && !x.deleted).ToList();
                if(checkArea == null || checkArea.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var pageList = new PageList<object>(loadDataAreaLineShelfByFloor(checkArea), page - 1, pageSize);
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

        private List<AreagetAllData> loadDataAreaLineShelfByFloor(List<areas> data)
        {
            var list = new List<AreagetAllData>();

            foreach(var item in data)
            {
                var dataItem = new AreagetAllData
                {
                    id = item.id,
                    name = item.name,
                    data = dataLineByArea(item.id)
                };

                list.Add(dataItem);
            }

            return list;
        }

        private List<dataListLineBtArea> dataLineByArea(int areaId)
        {
            var list = new List<dataListLineBtArea>();

            var checkLine = _context.linespage.Where(x => x.id_area == areaId && !x.deleted).ToList();
            foreach(var item in checkLine)
            {
                var dataItem = new dataListLineBtArea
                {
                    id = item.id,
                    name = item.name,
                    quantityshelf = item.quantityshelf,
                    shelfGetAlls = loadDataShelfByLine(item.id)
                };

                list.Add(dataItem);
            }

            return list;
        }

        private List<ShelfGetAll> loadDataShelfByLine(int line)
        {
            var list = new List<ShelfGetAll>();

            var checkShelf = _context.shelfs.Where(x => x.line == line && !x.deleted).ToList();
            foreach( var item in checkShelf)
            {
                list.Add(dataFindOneShelf(item));
            }

            return list;
        }
    }
}
