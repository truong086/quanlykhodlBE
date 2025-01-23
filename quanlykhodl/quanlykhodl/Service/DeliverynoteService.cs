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

                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Deliverynote>(data);
                mapData.code = RanDomCode.geneAction(8);
                mapData.account = checkAccount;
                mapData.accountmap = checkAccount.id;

                _context.deliverynotes.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.deliverynotes.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (data.isRetailcustomers)
                {
                    var createRetaiCustomer = new Retailcustomers
                    {
                        name = data.name,
                        email = data.email,
                        address = data.address,
                        phone = data.phone
                    };

                    _context.Retailcustomers.Add(createRetaiCustomer);
                    _context.SaveChanges();

                    var dataNewRetai = _context.Retailcustomers.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    dataNew.retailcustomers = dataNewRetai.id;
                    dataNew.retailcustomers_id = dataNewRetai;

                }
                else
                {
                    var checkRetailCustom = _context.Retailcustomers.Where(x => x.id == data.RetailcustomersOld && !x.Deleted).FirstOrDefault();
                    if (checkRetailCustom == null)
                        return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(Status.DATANULL));

                    dataNew.retailcustomers = checkRetailCustom.id;
                    dataNew.retailcustomers_id = checkRetailCustom;
                }

                if(data.products != null && data.products.Any())
                {
                    addDeliverynoteItem(data.products, dataNew);
                }

                _context.deliverynotes.Update(dataNew);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<DeliverynoteDTO>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<DeliverynoteDTO>.CreatedFail(ex.Message));
            }
        }

        private void addDeliverynoteItem(List<productDeliverynoteDTO> data, Deliverynote deliverynote)
        {
            foreach (var item in data)
            {
                var checkProduct = _context.prepareToExports.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                if (checkProduct != null)
                {
                    var addDataItem = new DeliverynotePrepareToExport
                    {
                       id_delivenote = deliverynote.id,
                       deliverynotes = deliverynote,
                       id_PrepareToExport = checkProduct.id,
                       PreparetoExports = checkProduct,
                       
                    };

                    checkProduct.Deleted = true;
                    _context.prepareToExports.Update(checkProduct);
                    _context.SaveChanges();

                    _context.deliverynotePrepareToEs.Add(addDataItem);
                    _context.SaveChanges();
                }
            }
        }
        private bool checkQuantity(List<productDeliverynoteDTO> data)
        {
            if(data.Any())
            {
                foreach(var item in data)
                {
                    var checkId = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
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
                var checkId = _context.deliverynotes.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.Deleted = true;

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
                var data = _context.deliverynotes.Where(x => !x.Deleted).ToList();

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
            var checkAccount = _context.accounts.Where(x => x.id == id.accountmap && !x.Deleted).FirstOrDefault();
            var checkRetaiCustomer = _context.Retailcustomers.Where(x => x.id == id.retailcustomers && !x.Deleted).FirstOrDefault();

            var checkTotalQuantityProduct = _context.deliverynotePrepareToEs.Where(x => x.id_delivenote == id.id && !x.Deleted).ToList();

            var mapData = _mapper.Map<DeliverynoteGetAll>(id);
            mapData.nameAccountCreat = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            mapData.ImageAccountCreat = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
            mapData.nameAccountBy = checkRetaiCustomer == null ? Status.ACCOUNTNOTFOULD : checkRetaiCustomer.name;
            mapData.EmailAccountBy = checkRetaiCustomer == null ? Status.ACCOUNTNOTFOULD : checkRetaiCustomer.email;
            mapData.AddressAccountBy = checkRetaiCustomer == null ? Status.ACCOUNTNOTFOULD : checkRetaiCustomer.address;
            mapData.products = loadProductImportData(id.id);
            mapData.TotalProduct = _context.deliverynotePrepareToEs.Where(x => x.id_delivenote == id.id && !x.Deleted).Count();
            mapData.TotalQuantity = sumTotal(checkTotalQuantityProduct);

            return mapData;
        }

        private long sumTotal(List<DeliverynotePrepareToExport> data)
        {
            long sum = 0;
            var list = new List<DeliverynotePrepareToExport>();

            foreach(var item in data)
            {
                var checkQuantity = _context.prepareToExports.Where(x => x.id == item.id && !x.Deleted).FirstOrDefault();
                if (checkQuantity != null)
                    sum += checkQuantity.quantity;
            }

            return sum;
        }

        private List<productImportformAndDeliveerrynote> loadProductImportData(int id)
        {
            var list = new List<productImportformAndDeliveerrynote>();
            //var checkProductImport = _context.productDeliverynotes.Where(x => x.deliverynote == id && !x.Deleted).ToList();
            var checkProductImport = _context.deliverynotePrepareToEs.Where(x => x.id_delivenote == id && !x.Deleted).ToList();
            if (checkProductImport != null)
            {
                foreach (var item in checkProductImport)
                {
                    var checkDelivenoteProduct = _context.prepareToExports.Where(x => x.id == item.id_PrepareToExport).FirstOrDefault();
                    if(checkDelivenoteProduct != null)
                    {
                        var checkProduct = _context.products1.Where(x => x.id == checkDelivenoteProduct.id_product && !x.Deleted).FirstOrDefault();
                        if (checkProduct != null)
                        {

                            var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.Deleted).FirstOrDefault();
                            var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.Deleted).FirstOrDefault();
                            var checkAccount = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();

                            var dataItem = _mapper.Map<productImportformAndDeliveerrynote>(checkProduct);
                            dataItem.id = checkProduct.id;
                            dataItem.account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
                            dataItem.account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
                            dataItem.category_map = checkCategory == null ? Status.ACCOUNTNOTFOULD : checkCategory.name;
                            dataItem.category_image = checkCategory == null ? Status.NOCATEGORY : checkCategory.image;
                            dataItem.suppliers = checkSupplier == null ? Status.DATANULL : checkSupplier.name;
                            dataItem.suppliersImage = checkSupplier == null ? Status.ACCOUNTNOTFOULD : checkSupplier.image;
                            dataItem.data = loadDataAreaProduct(checkProduct.id);

                            list.Add(dataItem);
                        }
                    }
                    
                }
            }
            return list;
        }

        private List<listArea> loadDataAreaProduct(int id)
        {
            var list = new List<listArea>();
            var checkproductLocationData = _context.productlocations.Where(x => x.id_product == id && !x.Deleted).ToList();
            if (checkproductLocationData != null)
            {
                foreach (var item in checkproductLocationData)
                {
                    var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == item.id_area && !x.Deleted).FirstOrDefault();
                    if (checkArea != null)
                    {
                        if (checkArea.floor_id != null)
                        {
                            var checkWarehourse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.Deleted).FirstOrDefault();
                            if (checkWarehourse != null)
                            {
                                var dataItem = new listArea
                                {
                                    location = item.location,
                                    area = checkArea.name,
                                    floor = checkArea.floor_id.name,
                                    warehourse = checkWarehourse.name
                                };

                                list.Add(dataItem);
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
                var checkId = _context.deliverynotes.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
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
                var checkId = _context.deliverynotes.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<ImportformUpdate>.CreatedFail(Status.DATANULL));

                checkId.isPercentage = data.isPercentage;
                checkId.total = data.total;
                checkId.Tax = data.Tax;

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
                var checkData = _context.deliverynotes.Where(x => x.code == code && !x.Deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(loadFindOneData(checkData)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
