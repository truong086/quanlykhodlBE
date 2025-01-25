using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.Net.NetworkInformation;

namespace quanlykhodl.Service
{
    public class StatusService : IStatusService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public StatusService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.warehousetransferstatuses.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrWhiteSpace(name))
                    data = data.Where(x => x.status.Contains(name)).ToList();

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

        private List<StatusGetAll> LoadData(List<Warehousetransferstatus> data)
        {
            var list = new List<StatusGetAll>();

            if (data.Any())
            {
                foreach (var item in data)
                {
                    var dataItem = FindOneData(item);

                    list.Add(dataItem);
                }
            }

            return list;
        }

        private List<StatusItemPlan> loadDataStatusItemImage(int id)
        {
            var list = new List<StatusItemPlan>();
            var checkStaustItem = _context.statusItems.Where(x => x.warehousetransferstatus == id && !x.Deleted).ToList();
            if (checkStaustItem.Any())
            {
                foreach(var item in checkStaustItem)
                {
                    var dataItem = new StatusItemPlan
                    {
                        id = item.id,
                        icon = item.icon,
                        title = item.title,
                        image = LoadImageStatus(item.id)
                    };

                    list.Add(dataItem);
                }
            }

            return list;
        }

        private List<string> LoadImageStatus(int id)
        {
            var list = new List<string>();

            var checkImageStatus = _context.imagestatusitems.Where(x => x.statusItemMap == id && !x.Deleted).ToList();
            if (checkImageStatus.Any())
            {
                foreach (var item in checkImageStatus)
                {
                    list.Add(item.image);
                }
            }
            return list;
        }

        public async Task<PayLoad<object>> FindByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkPlan = _context.plans.Where(x => x.Receiver == checkAccount.id && !x.Deleted).ToList();

                var pageList = new PageList<object>(loadDataFinOne(checkPlan), page - 1, pageSize);

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

        private List<StatusGetAll> loadDataFinOne(List<Plan> data)
        {
            var list = new List<StatusGetAll>();

            if (data.Any())
            {
                foreach (var item in data)
                {
                    var checkStatus = _context.warehousetransferstatuses.Where(x => x.plan == item.id && !x.Deleted).FirstOrDefault();
                    if(checkStatus != null)
                    {
                        list.Add(FindOneData(checkStatus));
                    }
                }
            }

            return list;
        }

        private StatusGetAll FindOneData(Warehousetransferstatus item)
        {
            var dataItem = new StatusGetAll();
            var checkPlan = _context.plans.Where(x => x.id == item.plan && !x.Deleted).FirstOrDefault();
            if(checkPlan != null )
            {
                if (!checkPlan.isWarehourse)
                {
                    var checkLocationProduct = _context.productlocations.Where(x => x.id == checkPlan.productlocation_map && !x.Deleted).FirstOrDefault();
                    if (checkLocationProduct != null)
                    {
                        var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.Deleted).FirstOrDefault();
                        if (checkProduct != null)
                        {
                            var checkImageProduct = _context.imageProducts.Where(x => x.productMap == checkProduct.id).FirstOrDefault();
                            if (checkImageProduct != null)
                            {
                                dataItem.id_product = checkProduct.id;
                                dataItem.product_iamge = checkImageProduct == null ? "Không có ảnh" : checkImageProduct.Link;
                                dataItem.product_name = checkProduct.title;
                            }
                        }
                    }

                }

                var checkAreaOld = _context.areas.Where(x => x.id == checkPlan.areaOld && !x.Deleted).FirstOrDefault();
                var checkFloorOld = _context.floors.Where(x => x.id == checkAreaOld.floor && !x.Deleted).FirstOrDefault();
                var checkWarehourseOld = _context.warehouses.Where(x => x.id == checkFloorOld.warehouse && !x.Deleted).FirstOrDefault();
                var checkAreaNew = _context.areas.Where(x => x.id == checkPlan.area && !x.Deleted).FirstOrDefault();
                var checkFloorNew = _context.floors.Where(x => x.id == checkAreaNew.floor && !x.Deleted).FirstOrDefault();
                var checkWarehourseNew = _context.warehouses.Where(x => x.id == checkFloorNew.warehouse && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == checkPlan.Receiver && !x.Deleted).FirstOrDefault();

                dataItem.id_status = item.id;
                dataItem.id_plan = checkPlan.id;
                dataItem.plan_tile = checkPlan.title;
                dataItem.areaNew = checkAreaNew.name;
                dataItem.areaOld = checkAreaOld.name;
                dataItem.FloorNew = checkFloorNew.name;
                dataItem.FloorOld = checkFloorOld.name;
                dataItem.WarehourseNew = checkWarehourseNew.name;
                dataItem.WarehourseOld = checkWarehourseOld.name;
                dataItem.StatusPlan = item.status;
                dataItem.Account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
                dataItem.Account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
                dataItem.statusItemPlans = loadDataStatusItemImage(item.id);
            }

            return dataItem;
        }

        public async Task<PayLoad<StatusWarehours>> UpdateStatus(StatusWarehours statusItemDTO)
        {
            try
            {
                var checkId = _context.warehousetransferstatuses.Where(x => x.id == statusItemDTO.id_statuswarehourse && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.DATANULL));

                var checkPlan = _context.plans.Where(x => x.id == checkId.plan && !x.Deleted).FirstOrDefault();
                if (checkPlan == null)
                    return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.DATANULL));

                if (statusItemDTO.title.ToLower() == Status.DONE.ToLower())
                {
                    var checkAreaNew = _context.areas.Where(x => x.id == checkPlan.area && !x.Deleted).FirstOrDefault();
                    var checkAreaOld = _context.areas.Where(x => x.id == checkPlan.areaOld && !x.Deleted).FirstOrDefault();
                    if (!checkPlan.isWarehourse)
                    {
                        var checkLocationProduct = _context.productlocations.Where(x => x.id == checkPlan.productlocation_map && !x.Deleted).FirstOrDefault();
                        var checkProductExxsis = _context.productlocations.Where(x => x.id_product == checkLocationProduct.id_product
                        && x.id_area == checkAreaNew.id && x.location == checkPlan.localtionNew && !x.Deleted && x.id != checkLocationProduct.id).FirstOrDefault();

                        if (checkProductExxsis != null)
                        {
                            var checkLocationNew = _context.productlocations.Where(x => x.id == checkProductExxsis.id && !x.Deleted).FirstOrDefault();
                            checkLocationNew.quantity += checkLocationProduct.quantity;
                            checkLocationProduct.Deleted = true;

                            _context.productlocations.Update(checkLocationNew);
                            _context.SaveChanges();
                        }
                        else
                        {
                            checkLocationProduct.location = checkPlan.localtionNew.Value;
                            checkLocationProduct.areas = checkAreaNew;
                            checkLocationProduct.id_area = checkAreaNew.id;
                        }

                        _context.productlocations.Update(checkLocationProduct);
                    }
                    else
                    {
                        var checkProductLocationOld = _context.productlocations.Where(x => x.id_area == checkAreaOld.id && x.location == checkPlan.localtionOld && !x.Deleted).ToList();
                        var checkProductLocationNew = _context.productlocations.Where(x => x.id_area == checkAreaNew.id && x.location == checkPlan.localtionOld && !x.Deleted).ToList();
                        if (checkProductLocationOld != null && checkAreaNew != null)
                            updateAreaNew(checkAreaNew, checkProductLocationOld);

                        if(checkProductLocationNew != null && checkAreaOld != null)
                            updateAreaNew(checkAreaOld, checkProductLocationNew);
                    }

                    checkPlan.status = Status.DONE.ToLower();
                    checkId.status = Status.DONE.ToLower();
                    checkId.Deleted = true;
                    checkPlan.Deleted = true;
                }
                else
                {
                    checkPlan.status = statusItemDTO.title.ToLower();
                    checkId.status = statusItemDTO.title.ToLower();
                }

                _context.plans.Update(checkPlan);
                _context.warehousetransferstatuses.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<StatusWarehours>.Successfully(statusItemDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(ex.Message));
            }
        }

        private void updateAreaNew(Area areaNew, List<productlocation> data)
        {
            foreach(var item in data)
            {
                if(areaNew.quantity < item.location)
                {
                    item.location = areaNew.quantity.Value;
                    item.areas = areaNew;
                    item.id_area = areaNew.id;

                }
                else
                {
                    item.areas = areaNew;
                    item.id_area = areaNew.id;
                }

                _context.productlocations.Update(item);
                _context.SaveChanges();
            }
        }

        private int checkProducExsis(List<productlocation> data, int id)
        {
            foreach(var item in data)
            {
                if(item.id_product == id)
                    return item.id;
            }

            return 0;
        }
        public async Task<PayLoad<StatusItemDTO>> UpdateStatusItem(StatusItemDTO statusItemDTO)
        {
            try
            {
                var checkId = _context.warehousetransferstatuses.Where(x => x.id == statusItemDTO.id_status && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<StatusItemDTO>.CreatedFail(Status.DATANULL));

                var statusItemData = _mapper.Map<StatusItem>(statusItemDTO);
                statusItemData.warehousetransferstatus = checkId.id;
                statusItemData.Warehousetransferstatus_id = checkId;

                _context.statusItems.Add(statusItemData);
                _context.SaveChanges();

                if(statusItemDTO.image != null)
                {
                    var checkData = _context.statusItems.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    UploadImageCloud(statusItemDTO.image, TokenViewModel.STATUSITEM + checkData.id.ToString(), checkData);
                }

                return await Task.FromResult(PayLoad<StatusItemDTO>.Successfully(statusItemDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<StatusItemDTO>.CreatedFail(ex.Message));
            }
        }

        private void UploadImageCloud(List<IFormFile> images, string folder, StatusItem data)
        {
            var list = new List<imagestatusitem>();
            foreach(var item in images)
            {
                uploadCloud.CloudInaryIFromAccount(item, folder, _cloud);
                var imageStatusItemData = new imagestatusitem
                {
                    image = uploadCloud.Link,
                    publicid = uploadCloud.publicId,
                    statusItem_id = data,
                    statusItemMap = data.id
                };

                list.Add(imageStatusItemData);
            }

            _context.imagestatusitems.AddRange(list);
            _context.SaveChanges();
        }
    }
}
