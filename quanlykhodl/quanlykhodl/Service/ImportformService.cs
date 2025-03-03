﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Drawing;
using Twilio.Rest.Trunking.V1;

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
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                var mapData = _mapper.Map<Importform>(data);
                mapData.account_id = checkAccount;
                mapData.account_idmap = checkAccount.id;
                mapData.isaction = false;
                _context.importforms.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.importforms.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                dataNew.code = RanDomCode.geneAction(8) + dataNew.id.ToString();
                if (data.isProductNew)
                {
                    if(data.productNew != null && data.productNew.Any())
                    {
                        if (!checkCategoryProduct(data.productNew))
                        {
                            _context.importforms.Remove(dataNew);
                            _context.SaveChanges();
                            return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(Status.NOCATEGORY));
                        }
                            

                        if (!AddProductLocation(data.productNew, checkAccount, dataNew))
                        {
                            _context.importforms.Remove(dataNew);
                            _context.SaveChanges();
                            return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(Status.FULLQUANTITY));
                        }
                    }
                }
                else
                {
                    if (data.productOlds != null && data.productOlds.Any())
                    {
                        if(!UpdataQuantityProduct(data.productOlds, dataNew))
                        {
                            _context.importforms.Remove(dataNew);
                            _context.SaveChanges();
                            return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(Status.FULLQUANTITY));
                        }
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
        //        var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
        //        var checkArea = _context.shelfs.Where(x => x.id == item.shelfId && !x.deleted).FirstOrDefault();
        //        var checkLocationArea = _context.productlocations.Where(x => x.location == item.location && x.id_shelf == checkArea.id && !x.deleted).FirstOrDefault();
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
        //                shelfs = checkArea,
        //                shelf_id = checkArea.id,
        //                location = item.location,
        //                isaction = false
        //            };

        //            checkLocationArea.quantity += item.quantity;
        //            _context.productlocations.Update(checkLocationArea);

        //            _context.productimportforms.Add(importProductData);

        //            _context.SaveChanges();
        //        }
        //    }
        //}

        private bool UpdataQuantityProduct(List<productOld> data, Importform importform)
        {
            foreach (var item in data)
            {
                if (!checkProductLocationOld(item))
                    return false;

                var checkSupplier = _context.suppliers.Where(x => x.id == item.supplier && !x.deleted).FirstOrDefault();
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                var checkArea = _context.shelfs.Where(x => x.id == item.shelfId && !x.deleted).FirstOrDefault();

                var checkLocationArea = _context.productlocations.Where(x => x.location == item.location && x.id_shelf == checkArea.id && !x.deleted && x.isaction).FirstOrDefault();
                if (!CheckQuantity.checkLocationQuantity(checkArea, item.location.Value, item.quantity, _context))
                    return false;

                if (checkProduct != null && checkLocationArea != null && checkSupplier != null)
                {
                    var importProductData = new productImportform
                    {
                        supplier = checkSupplier.id,
                        supplier_id = checkSupplier,
                        importform = importform.id,
                        importform_id1 = importform,
                        product = checkProduct.id,
                        products = checkProduct,
                        quantity = item.quantity,
                        shelfs = checkArea,
                        shelf_id = checkArea.id,
                        location = item.location,
                        isaction = false,
                        code = RanDomCode.geneAction(8) + importform.id.ToString()
                    };

                    _context.productimportforms.Add(importProductData);

                    _context.SaveChanges();
                }
            }

            return true;
        }

        private bool checkProductLocationOld(productOld data)
        {
            var checkLocation = _context.productlocations.Where(x => x.id_product == data.id_product && x.location == data.location && x.id_shelf == data.shelfId && !x.deleted && x.isaction).FirstOrDefault();
            if (checkLocation != null)
            {
                if (data.id_product != checkLocation.id_product)
                    return false;
            }
            return true;
        }
        private bool AddProductLocation(List<productNew> data, accounts account, Importform importform)
        {
            foreach (var item in data)
            {
                if (!checkProductLocation(item))
                    return false;

                var checkArea = _context.shelfs.Where(x => x.id == item.shelfId && !x.deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == item.suppliers && !x.deleted).FirstOrDefault();
                var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.deleted).FirstOrDefault();
                var checkName = _context.products1.Where(x => x.title == item.title && !x.deleted).FirstOrDefault();

                if(checkName != null)
                    return false;

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

                var dataNew = _context.products1.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
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
                    shelfs = checkArea,
                    id_shelf = checkArea.id,
                    id_product = dataNew.id,
                    location = item.location.Value,
                    products = dataNew,
                    quantity = item.quantityLocation,
                    isaction = false
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
                    shelfs = checkArea,
                    shelf_id = checkArea.id,
                    location = item.location,
                    isaction = false,
                    code = RanDomCode.geneAction(8) + importform.id.ToString()
                };

                _context.productimportforms.Add(productImportData);

                _context.SaveChanges();

            }

            return true;
        }

        private bool checkProductLocation(productNew item)
        {
            var checkLocation = _context.productlocations.Where(x => x.location == item.location && x.id_shelf == item.shelfId && !x.deleted).ToList();
            if (checkLocation != null && checkLocation.Any() && checkLocation.Count > 0)
            {
                return false;
            }
            return true;
        }
        //private bool AddProductLocation(List<productNew> data, accounts account, Importform importform)
        //{
        //    foreach(var item in data)
        //    {
        //        var checkArea = _context.shelfs.Where(x => x.id == item.shelfId && !x.deleted).FirstOrDefault();
        //        var checkSupplier = _context.suppliers.Where(x => x.id == item.suppliers && !x.deleted).FirstOrDefault();
        //        var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.deleted).FirstOrDefault();

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

        //        var dataNew = _context.products1.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
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
        //            shelfs = checkArea,
        //            id_shelf = checkArea.id,
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
        //            shelfs = checkArea,
        //            shelf_id = checkArea.id,
        //            location = item.location
        //        };

        //        _context.productimportforms.Add(productImportData);

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
                    link = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    productmap = dataPr.id,
                    products_id = dataPr
                };

                _context.imageproducts.Add(imageProduct);
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
                    link = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    productmap = dataPr.id,
                    products_id = dataPr
                };

                _context.imageproducts.Add(imageProduct);
                _context.SaveChanges();
            }
        }
        private bool checkCategoryProduct(List<productNew> data)
        {
            foreach(var item in data)
            {
                var checkProductLocation = _context.productlocations.Include(p => p.products).Where(x => x.id_shelf == item.shelfId && x.location == item.location && !x.deleted && x.isaction).FirstOrDefault();
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
                var checkId = _context.importforms.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;
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
                var data = _context.importforms.Where(x => !x.deleted).ToList();

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
            var checkAccount = _context.accounts.Where(x => x.id == item.account_idmap && !x.deleted).FirstOrDefault();
            var mapData = _mapper.Map<ImportformGetAll>(item);
            mapData.Id = item.id;
            mapData.accountName = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            mapData.accountImage = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
            mapData.products = loadProductImportData(item.id);
            mapData.totalProduct = _context.productimportforms.Where(x => x.importform == item.id && !x.deleted).Count();
            mapData.totalQuanTity = _context.productimportforms.Where(x => x.importform == item.id && !x.deleted).Sum(x => x.quantity);
            return mapData;
        }

        private List<productImportformAndDeliveerrynote> loadProductImportData(int id)
        {
            var list = new List<productImportformAndDeliveerrynote>();
            var checkProductImport = _context.productimportforms.Where(x => x.importform == id && !x.deleted).ToList();
            if(checkProductImport != null)
            {
                foreach(var item in checkProductImport)
                {
                    var checkProduct = _context.products1.Where(x => x.id == item.product && !x.deleted).FirstOrDefault();
                    if(checkProduct != null)
                    {
                        var checkCategory = _context.categories.Where(x => x.id == checkProduct.category_map && !x.deleted).FirstOrDefault();
                        var checkSupplier = _context.suppliers.Where(x => x.id == checkProduct.suppliers && !x.deleted).FirstOrDefault();
                        var checkAccount = _context.accounts.Where(x => x.id == checkProduct.account_map && !x.deleted).FirstOrDefault();

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

            var checkImageProduct = _context.imageproducts.Where(x => x.productmap == id && !x.deleted).ToList();
            if(checkImageProduct.Count > 0)
            {
                foreach(var item in checkImageProduct)
                {
                    list.Add(item.link);
                }
            }

            return list;
        }

        private listArea loadDataAreaProductAreaLocation(productImportform data)
        {
            var dataItem = new listArea();
            //var checkproductLocationData = _context.productlocations.Where(x => x.id_product == data.product && x.id_shelf == data.shelf_id && x.location == data.location && !x.deleted).FirstOrDefault();
            //if (checkproductLocationData != null)
            //{
                
            //}

            var checkShelf = _context.shelfs.Include(f => f.line_id).Where(x => x.id == data.shelf_id && !x.deleted).FirstOrDefault();
            if (checkShelf != null)
            {
                var checkCodeArea = _context.codelocations.Where(x => x.id_helf == checkShelf.id && x.location == data.location && !x.deleted).FirstOrDefault();
                var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                if (checkArea != null)
                {
                    if (checkArea.floor_id != null)
                    {
                        var checkWarehourse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.deleted).FirstOrDefault();
                        if (checkWarehourse != null)
                        {

                            dataItem.location = data.location;
                            dataItem.shelf = checkShelf.name;
                            dataItem.floor = checkArea.floor_id.name;
                            dataItem.area = checkArea.name;
                            dataItem.warehourse = checkWarehourse.name;
                            dataItem.code = checkCodeArea == null ? "No Code" : checkCodeArea.code;
                            dataItem.codeShelf = checkShelf == null ? "No Code" : checkShelf.code;
                            dataItem.isAction = data.isaction;
                        }
                    }
                }


            }

            return dataItem;
        }
        private List<listArea> loadDataAreaProduct(int id)
        {
            var list = new List<listArea>();
            var checkproductLocationData = _context.productlocations.Where(x => x.id_product == id && !x.deleted).ToList();
            if (checkproductLocationData != null)
            {
                foreach (var item in checkproductLocationData)
                {
                    var checkShelf = _context.shelfs.Include(f => f.line_id).Where(x => x.id == item.id_shelf && !x.deleted).FirstOrDefault();
                    if (checkShelf != null)
                    {
                        var checkCodeLocation = _context.codelocations.Where(x => x.id_helf == checkShelf.id && x.location == item.location && !x.deleted).FirstOrDefault();

                        var checkArea = _context.areas.Include(f => f.floor_id).Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                        if(checkArea != null)
                        {
                            if (checkArea.floor_id != null)
                            {
                                var checkWarehourse = _context.warehouses.Where(x => x.id == checkArea.floor_id.warehouse && !x.deleted).FirstOrDefault();
                                if (checkWarehourse != null)
                                {
                                    var dataItem = new listArea
                                    {
                                        area = checkArea.name,
                                        code = checkCodeLocation == null ? Status.CODEFAILD : checkCodeLocation.code,
                                        codeShelf = checkShelf == null ? Status.CODEFAILD : checkShelf.code,
                                        location = item.location,
                                        shelf = checkShelf.name,
                                        floor = checkArea.floor_id.name,
                                        warehourse = checkWarehourse.name,
                                        isAction = item.isaction
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
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.importforms.Where(x => x.id == id && !x.deleted).FirstOrDefault();

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
                var checkId = _context.importforms.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<ImportformUpdate>.CreatedFail(Status.DATANULL));

                checkId.ispercentage = data.isPercentage;
                checkId.tax = data.Tax;
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
                var checkData = _context.importforms.Where(x => x.code == code && !x.deleted).FirstOrDefault();
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
                var checkCode = _context.importforms.Where(x => x.code == code.code && !x.deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkCode.isaction = true;
                checkCode.actualquantity = code.ActualQuantity;
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
            var checkData = _context.productimportforms.Where(x => x.importform == data.id && !x.deleted).ToList();
            if(checkData.Any())
            {
                foreach(var item in checkData) 
                {
                    var checkLocation = _context.productlocations.Where(x => x.id_product == item.product 
                    && x.location == item.location && x.id_shelf == item.shelf_id && !x.deleted).FirstOrDefault();

                    if(checkLocation != null)
                    {
                        if (!data.isproductnew)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == item.product && !x.deleted).FirstOrDefault();
                            if (checkProduct != null)
                            {
                                checkProduct.quantity += item.quantity;
                                checkLocation.quantity += item.quantity;

                                _context.products1.Update(checkProduct);
                            }
                        }
                        checkLocation.isaction = true;
                        item.isaction = true;

                        _context.productimportforms.Update(item);
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
                var checkCode = _context.productimportforms.Where(x => x.code == code && !x.deleted).FirstOrDefault();
                if (checkCode == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                var checkImportFrom = _context.importforms.Where(x => x.id == checkCode.importform && !x.deleted).FirstOrDefault();
                if (checkImportFrom == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkProductLocation = _context.productlocations.Where(x => x.id_product == checkCode.product
                    && x.location == checkCode.location && x.id_shelf == checkCode.shelf_id &&
                    !x.deleted).FirstOrDefault();

                if (checkProductLocation == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                if (checkImportFrom.isproductnew)
                {
                    if (!checkProductLocation.isaction)
                        return await Task.FromResult(PayLoad<object>.Successfully(true));
                }
                else
                {
                    if(checkProductLocation.isaction)
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
