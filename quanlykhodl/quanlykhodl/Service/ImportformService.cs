﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

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
                mapData.code = RanDomCode.geneAction(8);
                mapData.account_id = checkAccount;
                mapData.account_idMap = checkAccount.id;

                _context.importforms.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.importforms.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if(data.isProductNew)
                {
                    if(data.productNew != null && data.productNew.Any())
                    {
                        if (!checkCategoryProduct(data.productNew))
                            return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(Status.NOCATEGORY));

                        AddProductLocation(data.productNew, checkAccount, dataNew);
                    }
                }
                else
                {
                    if (data.productOlds != null && data.productOlds.Any())
                    {
                        UpdataQuantityProduct(data.productOlds, dataNew);
                    }
                }
                return await Task.FromResult(PayLoad<ImportformDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ImportformDTO>.CreatedFail(ex.Message));
            }
        }

        private void UpdataQuantityProduct(List<productOld> data, Importform importform)
        {
            foreach(var item in data)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.Deleted).FirstOrDefault();
                if(checkProduct != null)
                {
                    checkProduct.quantity += item.quantity;
                    _context.products1.Update(checkProduct);

                    var importProductData = new productImportform
                    {
                        importform = importform.id,
                        importform_id1 = importform,
                        product = checkProduct.id,
                        products = checkProduct,
                        quantity = item.quantity,
                    };

                    _context.productImportforms.Add(importProductData);

                    _context.SaveChanges();
                }
            }
        }

        private void AddProductLocation(List<productNew> data, Account account, Importform importform)
        {
            foreach(var item in data)
            {
                var checkArea = _context.areas.Where(x => x.id == item.areaId && !x.Deleted).FirstOrDefault();
                var checkSupplier = _context.suppliers.Where(x => x.id == item.suppliers && !x.Deleted).FirstOrDefault();
                var checkCategory = _context.categories.Where(x => x.id == item.category_map && !x.Deleted).FirstOrDefault();

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
                if(item.image != null)
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
                    quantity = item.quantityLocation
                };

                _context.productlocations.Add(productLocationNew);
                _context.SaveChanges();

                var productImportData = new productImportform
                {
                    importform = importform.id,
                    importform_id1 = importform,
                    product = dataNew.id,
                    products = dataNew,
                    quantity = item.quantity,
                    supplier = checkSupplier.id,
                    supplier_id = checkSupplier
                };

                _context.productImportforms.Add(productImportData);

                _context.SaveChanges();
                
            }
        }

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
                var checkProductLocation = _context.productlocations.Include(p => p.products).Where(x => x.id_area == item.areaId && x.location == item.location && !x.Deleted).FirstOrDefault();
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
            return list;
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
    }
}
