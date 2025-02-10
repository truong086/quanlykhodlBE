using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.Xml.Linq;

namespace quanlykhodl.Service
{
    public class DeliverynoteService : IDeliverynoteService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public DeliverynoteService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<DeliverynoteDTO>> Add(DeliverynoteDTO data)
        {
            try
            {
                var user = _userService.name();
                if (!checkQuantity(data.products))
                    return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(Status.DATANULL));

                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Deliverynote>(data);
                mapData.account = checkAccount;
                mapData.accountmap = checkAccount.id;
                mapData.isaction = false;

                _context.deliverynotes.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.deliverynotes.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                if (data.isRetailcustomers)
                {
                    var createRetaiCustomer = new Retailcustomers
                    {
                        name = data.name,
                        email = data.email,
                        address = data.address,
                        phone = data.phone
                    };

                    _context.retailcustomers.Add(createRetaiCustomer);
                    _context.SaveChanges();

                    var dataNewRetai = _context.retailcustomers.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                    dataNew.retailcustomers = dataNewRetai.id;
                    dataNew.retailcustomers_id = dataNewRetai;
                }
                else
                {
                    var checkRetailCustom = _context.retailcustomers.Where(x => x.id == data.RetailcustomersOld && !x.deleted).FirstOrDefault();
                    if (checkRetailCustom == null)
                        return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(Status.DATANULL));

                    dataNew.retailcustomers = checkRetailCustom.id;
                    dataNew.retailcustomers_id = checkRetailCustom;
                }

                if(data.products != null && data.products.Any())
                {
                    if(!addDeliverynoteItem(data.products, dataNew))
                        return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(Status.DATAERROR));
                }

                dataNew.code = RanDomCode.geneAction(8) + dataNew.id.ToString();
                _context.deliverynotes.Update(dataNew);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<DeliverynoteDTO>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(ex.Message));
            }
        }

        private bool addDeliverynoteItem(List<productDeliverynoteDTO> data, Deliverynote deliverynote)
        {
            var code = RanDomCode.geneAction(8) + deliverynote.id.ToString();
            foreach (var item in data)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                if (checkProduct != null)
                {
                    if (checkProduct.quantity < item.quantity)
                        return false;

                    var addDataItem = new productDeliverynote
                    {
                       location = null,
                       shelfs = null,
                       shelf_id = null,
                       productlocations = null,
                       productlocation_id = null,
                       deliverynote = deliverynote.id,
                       deliverynote_id1 = deliverynote,
                       product = checkProduct,
                       product_map = checkProduct.id,
                       quantity = item.quantity,
                       code = code

                    };

                    _context.productdeliverynotes.Add(addDataItem);
                    _context.SaveChanges();
                }
            }

            return true;
        }
        private bool checkQuantity(List<productDeliverynoteDTO> data)
        {
            if(data.Any())
            {
                foreach(var item in data)
                {
                    var checkId = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                    if(checkId != null)
                    {
                        if (checkId.quantity < item.quantity)
                            return false;
                    }
                }
            }
            return true;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.deliverynotes.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;

                _context.deliverynotes.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> findAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<DeliverynoteGetAll> loadData(List<Deliverynote> data)
        {
            var list = new List<DeliverynoteGetAll>();

            if(data != null)
            {
                if (data.Any())
                {
                    foreach(var item in data)
                    {
                        list.Add(loadFindOneData(item));
                    }
                }
            }

            return list;
        }
        private DeliverynoteGetAll loadFindOneData(Deliverynote id)
        {
            var checkAccount = _context.accounts.Where(x => x.id == id.accountmap && !x.deleted).FirstOrDefault();
            var checkRetaiCustomer = _context.retailcustomers.Where(x => x.id == id.retailcustomers && !x.deleted).FirstOrDefault();

            //var checkTotalQuantityProduct = _context.productdeliverynotes.Where(x => x.deliverynote == id.id && !x.deleted).ToList();

            var mapData = _mapper.Map<DeliverynoteGetAll>(id);
            mapData.nameAccountCreat = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            mapData.ImageAccountCreat = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
            mapData.nameAccountBy = checkRetaiCustomer == null ? Status.ACCOUNTNOTFOULD : checkRetaiCustomer.name;
            mapData.EmailAccountBy = checkRetaiCustomer == null ? Status.ACCOUNTNOTFOULD : checkRetaiCustomer.email;
            mapData.AddressAccountBy = checkRetaiCustomer == null ? Status.ACCOUNTNOTFOULD : checkRetaiCustomer.address;
            mapData.products = loadProductImportData(id.id);
            mapData.TotalProduct = _context.productdeliverynotes.Where(x => x.deliverynote == id.id && !x.deleted).Count();
            //mapData.TotalQuantity = sumTotal(checkTotalQuantityProduct);
            mapData.TotalQuantity = _context.productdeliverynotes.Where(x => x.deliverynote == id.id && !x.deleted).Sum(x => x.quantity);

            return mapData;
        }

        private long sumTotal(List<productDeliverynote> data)
        {
            long sum = 0;

            foreach(var item in data)
            {
                var checkQuantity = _context.productdeliverynotes.Where(x => x.id == item.id && !x.deleted).FirstOrDefault();
                if (checkQuantity != null)
                    sum += checkQuantity.quantity;
            }

            return sum;
        }

        private List<productImportformAndDeliveerrynote> loadProductImportData(int id)
        {
            var list = new List<productImportformAndDeliveerrynote>();
            //var checkProductImport = _context.productdeliverynotes.Where(x => x.deliverynote == id && !x.deleted).ToList();
            var checkProductImport = _context.productdeliverynotes.Where(x => x.deliverynote == id && !x.deleted).ToList();
            if (checkProductImport != null)
            {
                foreach (var item in checkProductImport)
                {
                    var checkProduct = _context.products1.Where(x => x.id == item.product_map && !x.deleted).FirstOrDefault();
                    if (checkProduct != null)
                    {

                        var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.deleted).FirstOrDefault();
                        var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.deleted).FirstOrDefault();
                        var checkAccount = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.deleted).FirstOrDefault();

                        var dataItem = _mapper.Map<productImportformAndDeliveerrynote>(checkProduct);
                        dataItem.id = checkProduct.id;
                        dataItem.account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
                        dataItem.account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
                        dataItem.category_map = checkCategory == null ? Status.ACCOUNTNOTFOULD : checkCategory.name;
                        dataItem.category_image = checkCategory == null ? Status.NOCATEGORY : checkCategory.image;
                        dataItem.suppliers = checkSupplier == null ? Status.DATANULL : checkSupplier.name;
                        dataItem.suppliersImage = checkSupplier == null ? Status.ACCOUNTNOTFOULD : checkSupplier.image;
                        dataItem.data = loadDataAreaProduct(checkProduct.id);
                        dataItem.dataItem = loadDataAreaProductAreaLocation(item);
                        dataItem.id_productDelivenote = item.id;
                        dataItem.code_productDelivenote = item.code;
                        dataItem.quantityDeliverynote = item.quantity;

                        list.Add(dataItem);
                    }

                }
            }
            return list;
        }

        private listArea loadDataAreaProductAreaLocation(productDeliverynote data)
        {
            var dataItem = new listArea();
            var checkproductLocationData = _context.productlocations.Where(x => x.id_product == data.product_map && x.id_shelf == data.shelf_id && x.location == data.location && !x.deleted).FirstOrDefault();
            if (checkproductLocationData != null)
            {
                var checkShelf = _context.shelfs.Include(f => f.area_id).Where(x => x.id == checkproductLocationData.id_shelf && !x.deleted).FirstOrDefault();
                if (checkShelf != null)
                {
                    var checkCodeArea = _context.codelocations.Where(x => x.id_helf == checkShelf.id && x.location == checkproductLocationData.location && !x.deleted).FirstOrDefault();
                    var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == checkShelf.area && !x.deleted).FirstOrDefault();
                    if (checkArea != null)
                    {
                        if (checkArea.floor_id != null)
                        {
                            var checkWarehourse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.deleted).FirstOrDefault();
                            if (checkWarehourse != null)
                            {

                                dataItem.location = checkproductLocationData.location;
                                dataItem.shelf = checkShelf.name;
                                dataItem.floor = checkArea.floor_id.name;
                                dataItem.area = checkArea.name;
                                dataItem.warehourse = checkWarehourse.name;
                                dataItem.code = checkCodeArea == null ? "No Code" : checkCodeArea.code;
                                dataItem.codeShelf = checkShelf == null ? "No Code" : checkShelf.code;
                                dataItem.isAction = checkproductLocationData.isaction;
                            }
                        }
                    }


                }
            }

            return dataItem;
        }

        private List<listArea> loadDataAreaProduct(int id)
        {
            var list = new List<listArea>();
            var checkproductLocationData = _context.productlocations.Where(x => x.id_product == id && !x.deleted && x.isaction).ToList();
            if (checkproductLocationData != null)
            {
                foreach (var item in checkproductLocationData)
                {
                    var checkShelf = _context.shelfs.Include(f => f.area_id).Where(x => x.id == item.id_shelf && !x.deleted).FirstOrDefault();
                    if (checkShelf != null)
                    {
                        if (checkShelf.area_id != null)
                        {
                            var checkCodeLocation = _context.codelocations.Where(x => x.id_helf == checkShelf.id
                            && x.location == item.location && !x.deleted).FirstOrDefault();
                            var checkFloor = _context.floors.Where(x => x.id == checkShelf.area_id.floor && !x.deleted).FirstOrDefault();
                            if (checkFloor != null)
                            {
                                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                                if(checkWarehourse != null)
                                {
                                    var dataItem = new listArea
                                    {
                                        area = checkShelf.area_id.name,
                                        location = item.location,
                                        shelf = checkShelf.name,
                                        floor = checkFloor.name,
                                        warehourse = checkWarehourse.name,
                                        isAction = item.isaction,
                                        codeShelf = checkShelf.code,
                                        code = checkCodeLocation == null ? Status.CODEFAILD : checkCodeLocation.code
                                    };

                                    list.Add(dataItem);
                                }
                                
                            }
                        }

                    }
                }
            }

            return list;
        }
        public async Task<PayLoad<object>> findOneById(int id)
        {
            try
            {
                var checkId = _context.deliverynotes.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(loadFindOneData(checkId)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ImportformUpdate>> Update(int id, ImportformUpdate data)
        {
            try
            {
                var checkId = _context.deliverynotes.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<ImportformUpdate>.CreatedFail(Status.DATANULL));

                checkId.ispercentage = data.isPercentage;
                checkId.total = data.total;
                checkId.tax = data.Tax;

                _context.deliverynotes.Add(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ImportformUpdate>.Successfully(data));
            }catch(Exception ex )
            {
                return await Task.FromResult(PayLoad<ImportformUpdate>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneCode(string code)
        {
            try
            {
                var checkData = _context.deliverynotes.Where(x => x.code == code && !x.deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(loadFindOneData(checkData)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
        public async Task<PayLoad<object>> FindOneCodeProduct(string code)
        {
            try
            {
                var checkData = _context.products1.Where(x => x.code == code && !x.deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkdelive = _context.productdeliverynotes.Where(x => x.product_map == checkData.id && !x.deleted).ToList();
                if (checkdelive.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(loadDataCode(checkdelive)));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<DeliverynoteGetAll> loadDataCode(List<productDeliverynote> data)
        {
            var list = new List<DeliverynoteGetAll>();
            foreach(var item in data)
            {
                var checkDelive = _context.deliverynotes.Where(x => x.id == item.deliverynote && !x.deleted).FirstOrDefault();
                if(checkDelive != null)
                {
                    list.Add(loadFindOneData(checkDelive));
                }
            }

            return list;
        }

        public async Task<PayLoad<uploadDataLocationArea>> UpdateActionLocation(uploadDataLocationArea data)
        {
            try
            {
                if(data.products == null)
                    return await Task.FromResult(PayLoad<uploadDataLocationArea>.CreatedFail(Status.DATANULL));

                var checkDataQuantity = _context.productdeliverynotes.Where(x => x.deliverynote == data.id && !x.deleted).Count();
                if (checkDataQuantity < data.products.Count() || checkDataQuantity > data.products.Count())
                    return await Task.FromResult(PayLoad<uploadDataLocationArea>.CreatedFail(Status.DATANULL));


                var checkId = _context.deliverynotes.Where(x => (x.id == data.id || x.code == data.code) && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<uploadDataLocationArea>.CreatedFail(Status.DATANULL));

                if (!checkQuantityData(data.products, checkId))
                    return await Task.FromResult(PayLoad<uploadDataLocationArea>.CreatedFail(Status.LOCATIONORPRODDUCTFAILD));
                checkId.isaction = true;

                _context.deliverynotes.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<uploadDataLocationArea>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<uploadDataLocationArea>.CreatedFail(ex.Message));
            }
        }
        private bool checkQuantityData(List<UploadproductDeliverynoteDTO> data, Deliverynote deliverynote)
        {
            if (data.Any())
            {
                foreach(var item in data)
                {
                    var checkId = _context.productdeliverynotes.Where(x => x.deliverynote == deliverynote.id && x.product_map == item.id_product && !x.deleted).FirstOrDefault();
                    if(checkId == null) return false;

                    var checkLocation = _context.productlocations.Where(x => x.id_product == checkId.product_map 
                    && x.id_shelf == item.area && x.location == item.location && !x.deleted && x.isaction)
                        .FirstOrDefault();
                    if(checkLocation == null)
                        return false;

                    var checkShelf = _context.shelfs.Where(x => x.id == checkLocation.id_shelf && !x.deleted).FirstOrDefault();
                    if(checkShelf == null) return false;

                    var checkProduct = _context.products1.Where(x => x.id == checkId.product_map && !x.deleted).FirstOrDefault();
                    if(checkProduct == null) return false;

                    if(checkLocation.quantity < checkId.quantity) return false;
                    if(checkProduct.quantity < checkId.quantity) return false;

                    checkId.productlocations = checkLocation;
                    checkId.productlocation_id = checkLocation.id;
                    checkId.location = checkLocation.location;
                    checkId.shelfs = checkShelf;
                    checkId.shelf_id = checkShelf.id;
                    checkId.updatedat = DateTimeOffset.UtcNow;

                    checkLocation.quantity -= checkId.quantity;

                    checkProduct.quantity -= checkId.quantity;

                    _context.products1.Update(checkProduct);
                    _context.productdeliverynotes.Update(checkId);
                    _context.productlocations.Update(checkLocation);
                    _context.SaveChanges();
                }

                return true;
            }

            return false;
        }

        public async Task<PayLoad<object>> FindAccountDelivenote(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkDelivenote = _context.deliverynotes.Where(x => x.accountmap == checkAccount.id && !x.deleted).ToList();
                if(checkDelivenote == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var pageList = new PageList<object>(loadData(checkDelivenote), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> CheckPack(updatePack data)
        {
            try
            {
                var checkData = _context.deliverynotes.Where(x => x.id == data.id || x.code == data.code && !x.ispack && !x.deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkData.ispack = true;

                _context.deliverynotes.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindNoPack(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && !x.ispack).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOkPack(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && x.ispack).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOkPackNoIsAction(string? name, int page = 1, int pageSize = 0)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && x.ispack && x.isaction == false).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOkPackOkIsAction(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && x.ispack && x.isaction == true).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindNoPackOkIsAction(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && !x.ispack && x.isaction == true).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindNoPackNoIsAction(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && !x.ispack && x.isaction == false).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindNoAction(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && x.isaction == false).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOkAction(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.deliverynotes.Where(x => !x.deleted && x.isaction == true).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAccountNoPack(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                var data = _context.deliverynotes.Where(x => !x.deleted && !x.ispack && x.accountmap == checkAccount.id).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAccountOkPack(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                var data = _context.deliverynotes.Where(x => !x.deleted && x.ispack && x.accountmap == checkAccount.id).ToList();
                
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages,
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetWarehouseSalesPercentage()
        {
            try
            {
                var data = _context.productdeliverynotes.Include(s => s.shelfs).Where(x => !x.deleted && x.location != null && x.shelf_id != null).ToList();

                var totalImportProduct = _context.productimportforms.Where(x => x.isaction && !x.deleted).Sum(x => x.quantity);
                return await Task.FromResult(PayLoad<object>.Successfully(WarehousePercentageData(data, totalImportProduct).OrderByDescending(x => x.Percentage)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<WarehouseSalesPercentage> WarehousePercentageData(List<productDeliverynote> data, int totalDataImportFormProduct)
        {
            var list = new List<WarehouseSalesPercentage>();
            var listCheck = new List<object>();
            var total = totalDataImportFormProduct;

            foreach (var item in data)
            {
                var checkId = listCheck.Where(x => ((dynamic)x).id == item.shelf_id && ((dynamic)x).location == item.location).FirstOrDefault();
                if(checkId == null)
                {
                    var checkTotal = _context.productdeliverynotes.Where(x => x.shelf_id == item.shelf_id && x.location == item.location && !x.deleted).Sum(x => x.quantity);
                    var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == item.shelfs.area && !x.deleted).FirstOrDefault();
                    var checkWarehouse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.deleted).FirstOrDefault();

                    var checkCodeLocation = _context.codelocations.Where(x => x.id_helf == item.shelf_id && x.location == item.location && !x.deleted).FirstOrDefault();
                    var percentage = (double)checkTotal / total * 100;
                    var dataItem = new WarehouseSalesPercentage
                    {
                        idArea = checkArea == null ? 0 : checkArea.id,
                        areaName = checkArea == null ? Status.NOAREA : checkArea.name,
                        areaImage = checkArea == null ? Status.NOAREA : checkArea.image,

                        idWarerouse = checkWarehouse == null ? 0 : checkWarehouse.id,
                        warehouseName = checkWarehouse == null ? Status.NOWAREHOURSE : checkWarehouse.name,
                        warehouseImage = checkWarehouse == null ? Status.NOWAREHOURSE : checkWarehouse.image,
                        idFloor = checkArea.floor_id == null ? 0 : checkArea.floor_id.id,
                        floorImage = checkArea.floor_id == null ? Status.NOFLOOR : checkArea.floor_id.image,
                        floorName = checkArea.floor_id == null ? Status.NOFLOOR : checkArea.floor_id.name,

                        idShelf = item.shelfs.id,
                        shelfName = item.shelfs.name,
                        shelfImage = item.shelfs.image,
                        code = checkCodeLocation == null ? Status.CODEFAILD : checkCodeLocation.code,
                        location = item.location,
                        Percentage = percentage,
                    };

                    list.Add(dataItem);
                    listCheck.Add(new
                    {
                        id = item.shelf_id,
                        location = item.location
                    });
                }
            }

            return list;
        }
    }
}
