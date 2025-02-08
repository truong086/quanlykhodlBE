using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class PrepareToExportService : IPrepareToExportService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public PrepareToExportService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }

        public Task<PayLoad<PrepareToExportDTO>> Add(PrepareToExportDTO data)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<string>> Deletet(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindAccount(string? name, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindCode(string code, string? name, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindDataNoIsCheck(string? name, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindOneCode(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindOneId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<PrepareToExportDTO>> Udpate(int id, PrepareToExportDTO data)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<string>> UdpateCheck(int id)
        {
            throw new NotImplementedException();
        }
        //public async Task<PayLoad<PrepareToExportDTO>> Add(PrepareToExportDTO data)
        //{
        //    try
        //    {
        //        var user = _userService.name();
        //        var checkData = _context.products1.Where(x => x.id == data.id_product && !x.deleted).FirstOrDefault();
        //        var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();

        //        if (checkData == null || checkAccount == null)
        //            return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.DATANULL));

        //        if (checkData.quantity < data.quantity)
        //            return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.FULLQUANTITY));

        //        var dataItem = new PrepareToExport
        //        {
        //            id_product = checkData.id,
        //            product = checkData,
        //            quantity = data.quantity,
        //            account = checkAccount,
        //            account_id = checkAccount.id,
        //            code = RanDomCode.geneAction(8),
        //            ischeck = false
        //        };

        //        checkData.quantity -= data.quantity;

        //        _context.products1.Update(checkData);
        //        _context.SaveChanges();

        //        _context.prepareToExports.Add(dataItem);
        //        _context.SaveChanges();

        //        return await Task.FromResult(PayLoad<PrepareToExportDTO>.Successfully(data));
        //    }catch (Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<string>> Deletet(int id)
        //{
        //    try
        //    {
        //        var checkId = _context.prepareToExports.Where(x => x.id == id && !x.deleted).FirstOrDefault();
        //        if (checkId == null)
        //            return await Task.FromResult(PayLoad<string>.CreatedFail(status.DATANULL));

        //        checkId.deleted = true;

        //        _context.prepareToExports.Update(checkId);
        //        _context.SaveChanges();

        //        return await Task.FromResult(PayLoad<string>.Successfully(status.SUCCESS));
        //    }catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        //{
        //    try
        //    {
        //        var data = _context.prepareToExports.Where(x => !x.deleted).ToList();
        //        var mapData = loadData(data);

        //        if (!string.IsNullOrEmpty(name))
        //            mapData = mapData.Where(x => x.title.Contains(name)).ToList();

        //        var pageList = new PageList<object>(mapData, page - 1, pageSize);

        //        return await Task.FromResult(PayLoad<object>.Successfully(new
        //        {
        //            data = pageList,
        //            page,
        //            pageList.pageSize,
        //            pageList.totalCounts,
        //            pageList.totalPages
        //        }));

        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}

        //private List<PrepareToExportGetAll> loadData(List<PrepareToExport> data)
        //{
        //    var list = new List<PrepareToExportGetAll>();

        //    if(data.Count > 0)
        //    {
        //        foreach(var item in data)
        //        {
        //            var checkProduc = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
        //            var mapData = _mapper.Map<PrepareToExportGetAll>(checkProduc);
        //            mapData.id = item.id;
        //            mapData.id_product = checkProduc == null ? null : checkProduc.id;
        //            mapData.quantity = item.quantity;
        //            mapData.areaFloorWarehourseDelivenotes = loadDataAreaDelivenote(checkProduc.id);

        //            list.Add(mapData);
        //        }
        //    }

        //    return list;
        //}

        //private List<areaFloorWarehourseDelivenote> loadDataAreaDelivenote(int id)
        //{
        //    var list = new List<areaFloorWarehourseDelivenote>();

        //    var checkLocaionProduct = _context.productlocations.Where(x => x.id_product == id && !x.deleted && x.isaction).ToList();
        //    if(checkLocaionProduct.Count > 0)
        //    {
        //        foreach(var item in checkLocaionProduct)
        //        {
        //            var dataItem = loadFindOneData(item);
        //            list.Add(dataItem);
        //        }
        //    }

        //    return list;
        //}

        //private areaFloorWarehourseDelivenote loadFindOneData(productlocation data)
        //{
        //    var dataItem = new areaFloorWarehourseDelivenote();
        //    var checkArea = _context.shelfs.Where(x => x.id == data.id_shelf && !x.deleted).FirstOrDefault();
        //    if(checkArea != null)
        //    {
        //        var checkLocationCode = _context.codelocations.Where(x => x.id_shelf == checkArea.id && x.location == data.location && !x.deleted).FirstOrDefault();
        //        if(checkLocationCode != null)
        //        {
        //            dataItem.location = checkLocationCode.location;
        //            dataItem.codeLocation = checkLocationCode.code;

        //        }

        //        dataItem.shelfs = checkArea.name;
        //        var checkFloor = _context.floors.Where(x => x.id == checkArea.shelfs && !x.deleted).FirstOrDefault();
        //        if (checkFloor != null)
        //        {
        //            dataItem.shelfs = checkFloor.name;

        //            var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
        //            if (checkWarehourse != null)
        //                dataItem.warehourse = checkWarehourse.name;
        //        }
        //    }

        //    return dataItem;
        //}
        //public async Task<PayLoad<object>> FindOneId(int id)
        //{
        //    try
        //    {
        //        var checkId = _context.prepareToExports.Where(x => x.id == id && !x.deleted).FirstOrDefault();
        //        if (checkId == null)
        //            return await Task.FromResult(PayLoad<object>.CreatedFail(status.DATANULL));

        //        var checkProduc = _context.products1.Where(x => x.id == checkId.id_product && !x.deleted).FirstOrDefault();
        //        var mapData = _mapper.Map<PrepareToExportGetAll>(checkProduc);
        //        mapData.id = checkId.id;
        //        mapData.id_product = checkProduc == null ? null : checkProduc.id;
        //        mapData.quantity = checkId.quantity;
        //        mapData.areaFloorWarehourseDelivenotes = loadDataAreaDelivenote(checkProduc.id);

        //        return await Task.FromResult(PayLoad<object>.Successfully(mapData));
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<PrepareToExportDTO>> Udpate(int id, PrepareToExportDTO data)
        //{
        //    try
        //    {
        //        var user = _userService.name();
        //        var checkId = _context.prepareToExports.Where(x => x.id == id && !x.deleted).FirstOrDefault();
        //        var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
        //        if (checkId == null || checkAccount == null)
        //            return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.DATANULL));

        //        var checkproduct = _context.products1.Where(x => x.id == checkId.id_product && !x.deleted).FirstOrDefault();
        //        if (checkproduct == null)
        //            return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.DATANULL));

        //        if(checkId.id_product != data.id_product)
        //        {
        //            checkproduct.quantity += checkId.quantity;
        //            _context.products1.Update(checkproduct);
        //            _context.SaveChanges();

        //            var checkproductNew = _context.products1.Where(x => x.id == data.id_product && !x.deleted).FirstOrDefault();
        //            if (checkproductNew == null)
        //                return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.DATANULL));

        //            if (checkproductNew.quantity < data.quantity)
        //                return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.FULLQUANTITY));

        //            checkId.quantity = data.quantity;
        //            checkId.product = checkproductNew;
        //            checkId.id_product = checkproductNew.id;
        //        }
        //        else
        //        {
        //            if (checkproduct.quantity + checkId.quantity < data.quantity)
        //                return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(status.FULLQUANTITY));

        //            checkId.quantity += data.quantity;
        //        }

        //        checkId.cretoredit = checkAccount.username + " đã sửa bản ghi vào lúc " + DateTimeOffset.UtcNow;
        //        _context.prepareToExports.Update(checkId);

        //        _context.SaveChanges();

        //        return await Task.FromResult(PayLoad<PrepareToExportDTO>.Successfully(data));
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<PrepareToExportDTO>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<object>> FindOneCode(string id)
        //{
        //    try
        //    {
        //        var checkCode = _context.prepareToExports.Where(x => x.code == id && !x.deleted).FirstOrDefault();
        //        if (checkCode == null)
        //            return await Task.FromResult(PayLoad<object>.CreatedFail(status.DATANULL));

        //        var checkProduc = _context.products1.Where(x => x.id == checkCode.id_product && !x.deleted).FirstOrDefault();
        //        var mapData = _mapper.Map<PrepareToExportGetAll>(checkProduc);
        //        mapData.id = checkCode.id;
        //        mapData.id_product = checkProduc == null ? null : checkProduc.id;
        //        mapData.quantity = checkCode.quantity;
        //        mapData.areaFloorWarehourseDelivenotes = loadDataAreaDelivenote(checkProduc.id);

        //        return await Task.FromResult(PayLoad<object>.Successfully(mapData));
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<string>> UdpateCheck(int id)
        //{
        //    try
        //    {
        //        var checkdata = _context.prepareToExports.Where(x => x.id == id && !x.deleted).FirstOrDefault();
        //        if (checkdata == null)
        //            return await Task.FromResult(PayLoad<string>.CreatedFail(status.DATANULL));

        //        checkdata.ischeck = true;

        //        _context.prepareToExports.Update(checkdata);
        //        _context.SaveChanges();

        //        return await Task.FromResult(PayLoad<string>.Successfully(status.SUCCESS));

        //    }catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<object>> FindDataNoIsCheck(string? name, int page = 1, int pageSize = 20)
        //{
        //    try
        //    {
        //        var checkData = _context.prepareToExports.Where(x => !x.ischeck && !x.deleted).ToList();
        //        if (checkData.Count <= 0)
        //            return await Task.FromResult(PayLoad<object>.CreatedFail(status.DATANULL));

        //        var mapData = loadData(checkData);
        //        if (!string.IsNullOrEmpty(name))
        //            mapData = mapData.Where(x => x.title.Contains(name)).ToList();

        //        var pageList = new PageList<object>(mapData, page - 1, pageSize);

        //        return await Task.FromResult(PayLoad<object>.Successfully(new
        //        {
        //            data = pageList,
        //            page,
        //            pageList.pageSize,
        //            pageList.totalCounts,
        //            pageList.totalPages
        //        }));

        //    }catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(status.DATANULL));
        //    }
        //}

        //public async Task<PayLoad<object>> FindAccount(string? name, int page = 1, int pageSize = 20)
        //{
        //    try
        //    {
        //        var user = _userService.name();
        //        var checkAccount = _context.prepareToExports.Where(x => x.account_id == Convert.ToInt32(user) && x.ischeck).ToList();
        //        var mapData = loadData(checkAccount);

        //        if (!string.IsNullOrEmpty(name))
        //            mapData = mapData.Where(x => x.title.Contains(name)).ToList();

        //        var pageList = new PageList<object>(mapData, page - 1, pageSize);
        //        return await Task.FromResult(PayLoad<object>.Successfully(new
        //        {
        //            data = pageList,
        //            page,
        //            pageList.pageSize,
        //            pageList.totalCounts,
        //            pageList.totalPages

        //        }));
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}

        //public async Task<PayLoad<object>> FindCode(string code, string? name, int page = 1, int pageSize = 20)
        //{
        //    try
        //    {
        //        var checkCode = _context.deliverynotepreparetoes.Where(x => x.code == code && !x.deleted).ToList();
        //        if (checkCode.Count <= 0)
        //            return await Task.FromResult(PayLoad<object>.CreatedFail(status.DATANULL));

        //        var mapData = loadDataCode(checkCode);
        //        if (!string.IsNullOrEmpty(name))
        //            mapData = mapData.Where(x => x.title.Contains(name)).ToList();

        //        var pageList = new PageList<object>(mapData, page - 1, pageSize);
        //        return await Task.FromResult(PayLoad<object>.Successfully(new
        //        {
        //            data = pageList,
        //            page,
        //            pageList.pageSize,
        //            pageList.totalCounts,
        //            pageList.totalPages
        //        }));
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}

        //private List<PrepareToExportGetAll> loadDataCode(List<DeliverynotePrepareToExport> data)
        //{
        //    var list = new List<PrepareToExportGetAll>();

        //    foreach(var item in data)
        //    {
        //        var checkDateaItem = _context.prepareToExports.Where(x => x.id == item.id_preparetoexport && !x.deleted && x.ischeck).FirstOrDefault();
        //        if(checkDateaItem != null)
        //        {
        //            var checkProduc = _context.products1.Where(x => x.id == checkDateaItem.id_product && !x.deleted).FirstOrDefault();
        //            if(checkProduc != null)
        //            {
        //                var mapData = _mapper.Map<PrepareToExportGetAll>(checkProduc);
        //                mapData.id = item.id;
        //                mapData.id_product = checkProduc == null ? null : checkProduc.id;
        //                mapData.quantity = checkDateaItem.quantity;
        //                mapData.areaFloorWarehourseDelivenotes = loadDataAreaDelivenote(checkProduc.id);

        //                list.Add(mapData);
        //            }
        //        }
        //    }

        //    return list;
        //}
    }
}
