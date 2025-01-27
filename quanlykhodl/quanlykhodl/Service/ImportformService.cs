using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Drawing;

namespace quanlykhodl.Service
{
    public class ImportformService : IImportformService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        private KiemTraBase64 _kiemtrabase64;
        public ImportformService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService, KiemTraBase64 kiemtrabase64)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
            _kiemtrabase64 = kiemtrabase64;

        }
        public async Task<PayLoad<ImportformDTO>> Add(ImportformDTO data)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<Importform>(data);
                mapData.account_id = checkAccount;
                mapData.account_idMap = checkAccount.id;
                mapData.isAction = false;
                _context.importforms.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.importforms.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                dataNew.code = RanDomCode.geneAction(8) + dataNew.id.ToString();
                if (data.isProductNew)
                {
                    if(data.productNew != null && data.productNew.Any())
                    {
                        if (!checkCategoryProduct(data.productNew))
                            return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(Status.NOCATEGORY));

                        if (!AddProductLocation(data.productNew, checkAccount, dataNew))
                            return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(Status.FULLQUANTITY));
                    }
                }
                else
                {
                    if (data.productOlds != null && data.productOlds.Any())
                    {
                        UpdataQuantityProduct(data.productOlds, dataNew);
                    }
                }

                _context.importforms.Update(dataNew);
                _context.SaveChanges();
                return await Task.FromResult(PayLoad<ImportformDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(ex.Message));
            }
        }

        //private void UpdataQuantityProduct(List<productOld> data, Importform importform)
        //{
        //    foreach(var item in data)
        //    {
        //        var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
        //        var checkArea = _context.areas.Where(x => x.id == item.areaId && !x.Deleted).FirstOrDefault();
        //        var checkLocationArea = _context.productlocations.Where(x => x.location == item.location && x.id_area == checkArea.id && !x.Deleted).FirstOrDefault();
        //        if(checkProduct != null && checkLocationArea != null)
        //        {
        //            checkProduct.quantity += item.quantity;
        //            _context.products1.Update(checkProduct);

        //            var importProductData = new productImportform
        //            {
        //                importform = importform.id,
        //                importform_id1 = importform,
        //                product = checkProduct.id,
        //                products = checkProduct,
        //                quantity = item.quantity,
        //                area = checkArea,
        //                area_id = checkArea.id,
        //                location = item.location,
        //                isAction = false
        //            };

        //            checkLocationArea.quantity += item.quantity;
        //            _context.productlocations.Update(checkLocationArea);

        //            _context.productImportforms.Add(importProductData);

        //            _context.SaveChanges();
        //        }
        //    }
        //}

        private void UpdataQuantityProduct(List<productOld> data, Importform importform)
        {
            foreach (var item in data)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.id == item.areaId && !x.Deleted).FirstOrDefault();
                var checkLocationArea = _context.productlocations.Where(x => x.location == item.location && x.id_area == checkArea.id && !x.Deleted && x.isAction).FirstOrDefault();
                if (checkProduct != null && checkLocationArea != null)
                {
                    var importProductData = new productImportform
                    {
                        importform = importform.id,
                        importform_id1 = importform,
                        product = checkProduct.id,
                        products = checkProduct,
                        quantity = item.quantity,
                        area = checkArea,
                        area_id = checkArea.id,
                        location = item.location,
                        isAction = false,
                        code = RanDomCode.geneAction(8) + importform.id.ToString()
                    };

                    _context.productImportforms.Add(importProductData);

                    _context.SaveChanges();
                }
            }
        }
        private bool AddProductLocation(List<productNew> data, Account account, Importform importform)
        {
            foreach (var item in data)
            {
                var checkArea = _context.areas.Where(x => x.id == item.areaId && !x.Deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == item.suppliers && !x.Deleted).FirstOrDefault();
                var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.Deleted).FirstOrDefault();

                if (!CheckQuantity.checkLocationQuantity(checkArea, item.location.Value, item.quantityLocation, _context))
                    return false;
                var mapData = _mapper.Map<product>(item);
                mapData.code = RanDomCode.geneAction(8);
                mapData.account_map = account.id;
                mapData.account = account;
                mapData.suppliers = checkSupplier.id;
                mapData.supplier_id = checkSupplier;
                mapData.category_map = checkCategory.id;
                mapData.categoryid123 = checkCategory;

                _context.products1.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.products1.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (item.image != null)
                {
                    if (!_kiemtrabase64.kiemtra(item.image[0]))
                    {
                        AddImageProductString(item.image, dataNew);
                    }
                    else
                    {
                        var chuyenDoi = chuyenDoiIFromFileProduct(item.image);
                        AddImageProduct(chuyenDoi, dataNew);
                    }
                }

                var productLocationNew = new productlocation
                {
                    areas = checkArea,
                    id_area = checkArea.id,
                    id_product = dataNew.id,
                    location = item.location.Value,
                    products = dataNew,
                    quantity = item.quantityLocation,
                    isAction = false
                };

                _context.productlocations.Add(productLocationNew);

                var productImportData = new productImportform
                {
                    importform = importform.id,
                    importform_id1 = importform,
                    product = dataNew.id,
                    products = dataNew,
                    quantity = item.quantity,
                    supplier = checkSupplier.id,
                    supplier_id = checkSupplier,
                    area = checkArea,
                    area_id = checkArea.id,
                    location = item.location,
                    isAction = false,
                    code = RanDomCode.geneAction(8) + importform.id.ToString()
                };

                _context.productImportforms.Add(productImportData);

                _context.SaveChanges();

            }

            return true;
        }
        //private bool AddProductLocation(List<productNew> data, Account account, Importform importform)
        //{
        //    foreach(var item in data)
        //    {
        //        var checkArea = _context.areas.Where(x => x.id == item.areaId && !x.Deleted).FirstOrDefault();
        //        var checkSupplier = _context.suppliers.Where(x => x.id == item.suppliers && !x.Deleted).FirstOrDefault();
        //        var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.Deleted).FirstOrDefault();

        //        if (!CheckQuantity.checkLocationQuantity(checkArea, item.location.Value, item.quantityLocation, _context))
        //            return false;
        //        var mapData = _mapper.Map<product>(item);
        //        mapData.code = RanDomCode.geneAction(8);
        //        mapData.account_map = account.id;
        //        mapData.account = account;
        //        mapData.suppliers = checkSupplier.id;
        //        mapData.supplier_id = checkSupplier;
        //        mapData.category_map = checkCategory.id;
        //        mapData.categoryid123 = checkCategory;

        //        _context.products1.Add(mapData);
        //        _context.SaveChanges();

        //        var dataNew = _context.products1.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        //        if(item.image != null)
        //        {
        //            if (!_kiemtrabase64.kiemtra(item.image[0]))
        //            {
        //                AddImageProductString(item.image, dataNew);
        //            }
        //            else
        //            {
        //                var chuyenDoi = chuyenDoiIFromFileProduct(item.image);
        //                AddImageProduct(chuyenDoi, dataNew);
        //            }
        //        }

        //        var productLocationNew = new productlocation
        //        {
        //            areas = checkArea,
        //            id_area = checkArea.id,
        //            id_product = dataNew.id,
        //            location = item.location.Value,
        //            products = dataNew,
        //            quantity = item.quantityLocation
        //        };

        //        _context.productlocations.Add(productLocationNew);
        //        _context.SaveChanges();

        //        var productImportData = new productImportform
        //        {
        //            importform = importform.id,
        //            importform_id1 = importform,
        //            product = dataNew.id,
        //            products = dataNew,
        //            quantity = item.quantity,
        //            supplier = checkSupplier.id,
        //            supplier_id = checkSupplier,
        //            area = checkArea,
        //            area_id = checkArea.id,
        //            location = item.location
        //        };

        //        _context.productImportforms.Add(productImportData);

        //        _context.SaveChanges();

        //    }

        //    return true;
        //}

        private void AddImageProduct(List<IFormFile> data, product dataPr)
        {
            foreach(var item in data)
            {
                uploadCloud.CloudInaryIFromAccount(item, TokenViewModel.PRODUCT + dataPr.id.ToString(), _cloud);
                var imageProduct = new ImageProduct
                {
                    Link = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    productMap = dataPr.id,
                    products_id = dataPr
                };

                _context.imageProducts.Add(imageProduct);
                _context.SaveChanges();
            }
        }

        private List<IFormFile> chuyenDoiIFromFileProduct(List<string> data)
        {
            var chuyenDoiStringBase64 = new ChuyenFile();
            var list = new List<IFormFile>();
            var sum = 0;
            foreach (var item in data)
            {
                //var fileName = "Product" + sum + ".jpg";
                var fileName = "Product" + sum;
                var dataItem = chuyenDoiStringBase64.chuyendoi(item, fileName);
                list.Add(dataItem);
                sum++;
            }

            return list;
        }
        private void AddImageProductString(List<string> data, product dataPr)
        {
            foreach (var item in data)
            {
                uploadCloud.CloudInaryAccount(item, TokenViewModel.PRODUCT + dataPr.id.ToString(), _cloud);
                var imageProduct = new ImageProduct
                {
                    Link = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    productMap = dataPr.id,
                    products_id = dataPr
                };

                _context.imageProducts.Add(imageProduct);
                _context.SaveChanges();
            }
        }
        private bool checkCategoryProduct(List<productNew> data)
        {
            foreach(var item in data)
            {
                var checkProductLocation = _context.productlocations.Include(p => p.products).Where(x => x.id_area == item.areaId && x.location == item.location && !x.Deleted && x.isAction).FirstOrDefault();
                if(checkProductLocation != null)
                {
                    if(checkProductLocation.products != null)
                        if (checkProductLocation.products.category_map != item.category_map)
                            return false;
                }
            }

            return true;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.importforms.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.Deleted = true;
                _context.importforms.Update(checkId);

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
                var data = _context.importforms.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.tite.Contains(name)).ToList();

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

        private List<ImportformGetAll> LoadData(List<Importform> data)
        {
            var list = new List<ImportformGetAll>();

            foreach(var item in data)
            {
                list.Add(LoadOneData(item));

            }
            return list;
        }

        private ImportformGetAll LoadOneData(Importform item)
        {
            var checkAccount = _context.accounts.Where(x => x.id == item.account_idMap && !x.Deleted).FirstOrDefault();
            var mapData = _mapper.Map<ImportformGetAll>(item);
            mapData.Id = item.id;
            mapData.accountName = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            mapData.accountImage = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
            mapData.products = loadProductImportData(item.id);
            mapData.totalProduct = _context.productImportforms.Where(x => x.importform == item.id && !x.Deleted).Count();
            mapData.totalQuanTity = _context.productImportforms.Where(x => x.importform == item.id && !x.Deleted).Sum(x => x.quantity);
            return mapData;
        }

        private List<productImportformAndDeliveerrynote> loadProductImportData(int id)
        {
            var list = new List<productImportformAndDeliveerrynote>();
            var checkProductImport = _context.productImportforms.Where(x => x.importform == id && !x.Deleted).ToList();
            if(checkProductImport != null)
            {
                foreach(var item in checkProductImport)
                {
                    var checkProduct = _context.products1.Where(x => x.id == item.product && !x.Deleted).FirstOrDefault();
                    if(checkProduct != null)
                    {
                        var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.Deleted).FirstOrDefault();
                        var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.Deleted).FirstOrDefault();
                        var checkAccount = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.Deleted).FirstOrDefault();

                        var dataItem = _mapper.Map<productImportformAndDeliveerrynote>(checkProduct);
                        dataItem.id = checkProduct.id;
                        dataItem.image = loadImageProduct(checkProduct.id);
                        dataItem.account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
                        dataItem.account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
                        dataItem.category_map = checkCategory == null ? Status.ACCOUNTNOTFOULD : checkCategory.name;
                        dataItem.category_image = checkCategory == null ? Status.NOCATEGORY : checkCategory.image;
                        dataItem.suppliers = checkSupplier == null ? Status.DATANULL : checkSupplier.name;
                        dataItem.suppliersImage = checkSupplier == null ? Status.ACCOUNTNOTFOULD : checkSupplier.image;
                        dataItem.data = loadDataAreaProduct(checkProduct.id);
                        dataItem.dataItem = loadDataAreaProductAreaLocation(item);
                        dataItem.codeProductImport = item.code == null ? Status.DATANULL : item.code;
                        dataItem.quantityImportFrom = item.quantity;

                        list.Add(dataItem);
                    }
                }
            }
            return list;
        }

        private List<string> loadImageProduct(int id)
        {
            var list = new List<string>();

            var checkImageProduct = _context.imageProducts.Where(x => x.productMap == id && !x.Deleted).ToList();
            if(checkImageProduct.Count > 0)
            {
                foreach(var item in checkImageProduct)
                {
                    list.Add(item.Link);
                }
            }

            return list;
        }

        private listArea loadDataAreaProductAreaLocation(productImportform data)
        {
            var dataItem = new listArea();
            var checkproductLocationData = _context.productlocations.Where(x => x.id_product == data.product && x.id_area == data.area_id && x.location == data.location && !x.Deleted).FirstOrDefault();
            if (checkproductLocationData != null)
            {
                var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == checkproductLocationData.id_area && !x.Deleted).FirstOrDefault();
                if (checkArea != null)
                {
                    var checkCodeArea = _context.codelocations.Where(x => x.id_area == checkArea.id && x.location == checkproductLocationData.location && !x.Deleted).FirstOrDefault();
                    if (checkArea.floor_id != null)
                    {
                        var checkWarehourse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.Deleted).FirstOrDefault();
                        if (checkWarehourse != null)
                        {

                            dataItem.location = checkproductLocationData.location;
                            dataItem.area = checkArea.name;
                            dataItem.floor = checkArea.floor_id.name;
                            dataItem.warehourse = checkWarehourse.name;
                            dataItem.code = checkCodeArea == null ? Status.CODEFAILD : checkCodeArea.code;
                            dataItem.isAction = checkproductLocationData.isAction;
                        }
                    }

                }
            }

            return dataItem;
        }
        private List<listArea> loadDataAreaProduct(int id)
        {
            var list = new List<listArea>();
            var checkproductLocationData = _context.productlocations.Where(x => x.id_product == id && !x.Deleted).ToList();
            if(checkproductLocationData != null)
            {
                foreach (var item in checkproductLocationData)
                {
                    var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == item.id_area && !x.Deleted).FirstOrDefault();
                    if(checkArea != null)
                    {
                        if(checkArea.floor_id != null)
                        {
                            var checkWarehourse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.Deleted).FirstOrDefault();
                            if (checkWarehourse != null)
                            {
                                var dataItem = new listArea
                                {
                                    location = item.location,
                                    area = checkArea.name,
                                    floor = checkArea.floor_id.name,
                                    warehourse = checkWarehourse.name,
                                    isAction = item.isAction
                                };

                                list.Add(dataItem);
                            }
                        }
                            
                    }
                }
            }

            return list;
        }
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.importforms.Where(x => x.id == id && !x.Deleted).FirstOrDefault();

                if (checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = LoadOneData(checkId)
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ImportformUpdate>> Update(int id, ImportformUpdate data)
        {
            try
            {
                var checkId = _context.importforms.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<ImportformUpdate>.CreatedFail(Status.DATANULL));

                checkId.isPercentage = data.isPercentage;
                checkId.Tax = data.Tax;
                checkId.total = data.total;

                _context.importforms.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ImportformUpdate>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ImportformUpdate>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindCode(string code)
        {
            try
            {
                var checkData = _context.importforms.Where(x => x.code == code && !x.Deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(LoadOneData(checkData)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> UpdateCode(ImportformUpdateCode code)
        {
            try
            {
                var checkCode = _context.importforms.Where(x => x.code == code.code && !x.Deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkCode.isAction = true;
                checkCode.ActualQuantity = code.ActualQuantity;
                _context.importforms.Update(checkCode);
                _context.SaveChanges();

                updateLocation(checkCode);

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        private void updateLocation(Importform data)
        {
            var checkData = _context.productImportforms.Where(x => x.importform == data.id && !x.Deleted).ToList();
            if(checkData.Any())
            {
                foreach(var item in checkData) 
                {
                    var checkLocation = _context.productlocations.Where(x => x.id_product == item.product 
                    && x.location == item.location && x.id_area == item.area_id && !x.Deleted).FirstOrDefault();

                    if(checkLocation != null)
                    {
                        if (!data.isProductNew)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == item.product && !x.Deleted).FirstOrDefault();
                            if (checkProduct != null)
                            {
                                checkProduct.quantity += item.quantity;
                                checkLocation.quantity += item.quantity;

                                _context.products1.Update(checkProduct);
                            }
                        }
                        checkLocation.isAction = true;
                        item.isAction = true;

                        _context.productImportforms.Update(item);
                        _context.productlocations.Update(checkLocation);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public async Task<PayLoad<object>> FindCodeProductImportFrom(string code)
        {
            try
            {
                var checkCode = _context.productImportforms.Where(x => x.code == code && !x.Deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                var checkImportFrom = _context.importforms.Where(x => x.id == checkCode.importform && !x.Deleted).FirstOrDefault();
                if (checkImportFrom == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkProductLocation = _context.productlocations.Where(x => x.id_product == checkCode.product
                    && x.location == checkCode.location && x.id_area == checkCode.area_id &&
                    !x.Deleted).FirstOrDefault();

                if (checkProductLocation == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                if (checkImportFrom.isProductNew)
                {
                    if (!checkProductLocation.isAction)
                        return await Task.FromResult(PayLoad<object>.Successfully(true));
                }
                else
                {
                    if(checkProductLocation.isAction)
                        return await Task.FromResult(PayLoad<object>.Successfully(true));
                }
                return await Task.FromResult(PayLoad<object>.Successfully(false));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
