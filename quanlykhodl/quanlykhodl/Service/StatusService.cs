using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using quanlykhodl.ChatHub;
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
        private readonly IHubContext<NotificationHub> _hubContext;
        public StatusService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
            _hubContext = hubContext;
        }
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.warehousetransferstatuses.Where(x => !x.deleted).ToList();

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
            var checkStaustItem = _context.statusitems.Where(x => x.warehousetransferstatus == id && !x.deleted).ToList();
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

            var checkImageStatus = _context.imagestatusitems.Where(x => x.statusitemmap == id && !x.deleted).ToList();
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
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkPlan = _context.plans.Where(x => x.Receiver == checkAccount.id && !x.deleted).ToList();

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
                    var checkStatus = _context.warehousetransferstatuses.Where(x => x.plan == item.id && !x.deleted).FirstOrDefault();
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
            var checkPlan = _context.plans.Where(x => x.id == item.plan && !x.deleted).FirstOrDefault();
            if(checkPlan != null )
            {
                if (!checkPlan.iswarehourse)
                {
                    var checkLocationProduct = _context.productlocations.Where(x => x.id == checkPlan.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
                    if (checkLocationProduct != null)
                    {
                        var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.deleted).FirstOrDefault();
                        if (checkProduct != null)
                        {
                            var checkImageProduct = _context.imageproducts.Where(x => x.productmap == checkProduct.id).FirstOrDefault();
                            if (checkImageProduct != null)
                            {
                                dataItem.id_product = checkProduct.id;
                                dataItem.product_iamge = checkImageProduct == null ? "Không có ảnh" : checkImageProduct.link;
                                dataItem.product_name = checkProduct.title;
                            }
                        }
                    }
                }

                var checkCodeLocationOld = _context.codelocations.Where(x => x.id_helf == checkPlan.shelfOld && x.location == checkPlan.localtionold && !x.deleted).FirstOrDefault();
                var checkCodeLocationNew = _context.codelocations.Where(x => x.id_helf == checkPlan.shelf && x.location == checkPlan.localtionnew && !x.deleted).FirstOrDefault();

                var checkShelfOld = _context.shelfs.Where(x => x.id == checkPlan.shelfOld && !x.deleted).FirstOrDefault();
                var checkAreaOld = _context.areas.Where(x => x.id == checkShelfOld.line && !x.deleted).FirstOrDefault();
                var checkFloorOld = _context.floors.Where(x => x.id == checkAreaOld.floor && !x.deleted).FirstOrDefault();
                var checkWarehourseOld = _context.warehouses.Where(x => x.id == checkFloorOld.warehouse && !x.deleted).FirstOrDefault();
                
                var checkShelfNew = _context.shelfs.Where(x => x.id == checkPlan.shelf && !x.deleted).FirstOrDefault();
                var checkAreaNew = _context.areas.Where(x => x.id == checkPlan.area && !x.deleted).FirstOrDefault();
                var checkFloorNew = _context.floors.Where(x => x.id == checkAreaNew.floor && !x.deleted).FirstOrDefault();
                var checkWarehourseNew = _context.warehouses.Where(x => x.id == checkFloorNew.warehouse && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == checkPlan.Receiver && !x.deleted).FirstOrDefault();

                dataItem.id_status = item.id;
                dataItem.id_plan = checkPlan.id;
                dataItem.plan_tile = checkPlan.title;
                dataItem.locationNew = checkPlan.localtionnew.Value;
                dataItem.locationOld = checkPlan.localtionold.Value;
                dataItem.shelfOld = checkShelfOld.name;
                dataItem.shelfNew = checkShelfNew.name;
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
                dataItem.CodeLocationNew = checkCodeLocationNew == null ? Status.CODEFAILD : checkCodeLocationNew.code;
                dataItem.CodeLocationOld = checkCodeLocationOld == null ? Status.CODEFAILD : checkCodeLocationOld.code;
            }

            return dataItem;
        }

        public async Task<PayLoad<StatusWarehours>> UpdateStatus(StatusWarehours statusItemDTO)
        {
            try
            {
                var user = _userService.name();
                var checkId = _context.warehousetransferstatuses.Where(x => x.id == statusItemDTO.id_statuswarehourse && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.DATANULL));
               
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.DATANULL));

                var checkPlan = _context.plans.Where(x => x.id == checkId.plan && !x.deleted && x.isconfirmation).FirstOrDefault();
                if (checkPlan == null)
                    return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.DATANULL));

                if(checkPlan.Receiver != checkAccount.id)
                    return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.ACCOUNTFAILD));

                if (statusItemDTO.title.ToLower() == Status.DONE.ToLower())
                {
                    var checkShelfNew = _context.shelfs.Where(x => x.id == checkPlan.shelf && !x.deleted).FirstOrDefault();
                    var checkShelOld = _context.shelfs.Where(x => x.id == checkPlan.shelfOld && !x.deleted).FirstOrDefault();
                    if (!checkPlan.iswarehourse)
                    {
                        var checkLocationProduct = _context.productlocations.Where(x => x.id == checkPlan.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
                        var checkProductExxsis = _context.productlocations.Where(x => x.id_product == checkLocationProduct.id_product
                        && x.id_shelf == checkShelfNew.id && x.location == checkPlan.localtionnew && !x.deleted && x.id != checkLocationProduct.id && x.isaction).FirstOrDefault();

                        if (checkProductExxsis != null)
                        {
                            var checkLocationNew = _context.productlocations.Where(x => x.id == checkProductExxsis.id && !x.deleted && x.isaction).FirstOrDefault();
                            if (checkLocationNew == null || checkLocationProduct == null)
                                return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(Status.DATANULL));

                            checkLocationNew.quantity += checkLocationProduct.quantity;
                            checkLocationProduct.deleted = true;

                            _context.productlocations.Update(checkLocationNew);
                            _context.SaveChanges();
                        }
                        else
                        {
                            checkLocationProduct.location = checkPlan.localtionnew.Value;
                            checkLocationProduct.shelfs = checkShelfNew;
                            checkLocationProduct.id_shelf = checkShelfNew.id;
                        }

                        _context.productlocations.Update(checkLocationProduct);
                    }
                    else
                    {
                        var checkProductLocationOld = _context.productlocations.Where(x => x.id_shelf == checkShelOld.id && x.location == checkPlan.localtionold && !x.deleted && x.isaction).ToList();
                        var checkProductLocationNew = _context.productlocations.Where(x => x.id_shelf == checkShelfNew.id && x.location == checkPlan.localtionnew && !x.deleted && x.isaction).ToList();
                        if (checkProductLocationOld != null && checkShelfNew != null)
                            updateAreaNew(checkShelfNew, checkProductLocationOld, checkPlan.localtionnew.Value);

                        if(checkProductLocationNew != null && checkShelOld != null)
                            updateAreaNew(checkShelOld, checkProductLocationNew, checkPlan.localtionold.Value);

                        addProductHistory(checkProductLocationOld, checkProductLocationNew, checkPlan);
                    }

                    checkPlan.status = Status.DONE.ToLower();
                    checkId.status = Status.DONE.ToLower();
                    checkId.deleted = true;
                    checkPlan.deleted = true;

                    await _hubContext.Clients.All.SendAsync("DonePlan", checkPlan.title, checkAccount.username);
                }
                else
                {
                    checkPlan.status = statusItemDTO.title.ToLower();
                    checkId.status = statusItemDTO.title.ToLower();
                }

                checkPlan.updatedat = DateTimeOffset.UtcNow;
                _context.plans.Update(checkPlan);
                _context.warehousetransferstatuses.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<StatusWarehours>.Successfully(statusItemDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<StatusWarehours>.CreatedFail(ex.Message));
            }
        }

        private void addProductHistory(List<productlocation> productlocationsNew, List<productlocation> productlocationsOld, Plan plan)
        {
            var listNew = new List<producthisstorylocation>();
            var listOld = new List<producthisstorylocation>();
            foreach (var item in productlocationsNew)
            {
                var checkproduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();

                var dataItem = new producthisstorylocation
                {
                    plans = plan,
                    plan_id = plan.id,
                    locationnew = item.location,
                    shelfnew = item.id_shelf,
                    products = checkproduct == null ? null : checkproduct,
                    product_id = checkproduct == null ? null : checkproduct.id
                };

                listNew.Add(dataItem);
            }

            _context.producthisstorylocations.AddRange(listNew);
            _context.SaveChanges();

            foreach(var item in productlocationsOld)
            {
                var checkProduct = _context.products1.Where(x => x.id == item.id_product && !x.deleted).FirstOrDefault();
                var dataItem = new producthisstorylocation
                {
                    plans = plan,
                    plan_id = plan.id,
                    locationold = item.location,
                    shelfold = item.id_shelf,
                    product_old = checkProduct == null ? null : checkProduct.id
                };

                listOld.Add(dataItem);

            }

            if(listOld.Count > 0)
            {
                _context.producthisstorylocations.AddRange(listOld);
                _context.SaveChanges();
            }
        }
        private void updateAreaNew(Shelf shelf, List<productlocation> data, int location)
        {
            foreach(var item in data)
            {
                if(shelf.quantity < item.location)
                {
                    item.location = shelf.quantity.Value;
                    item.shelfs = shelf;
                    item.id_shelf = shelf.id;

                }
                else
                {
                    item.location = location;
                    item.shelfs = shelf;
                    item.id_shelf = shelf.id;
                }

                item.updatedat = DateTimeOffset.UtcNow;
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
                var user = _userService.name();
                var checkId = _context.warehousetransferstatuses.Where(x => x.id == statusItemDTO.id_status && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<StatusItemDTO>.CreatedFail(Status.DATANULL));
                var checkPlan = _context.plans.Where(x => x.id == checkId.plan && !x.deleted && x.isconfirmation).FirstOrDefault();
                if (checkPlan == null)
                    return await Task.FromResult(PayLoad<StatusItemDTO>.CreatedFail(Status.DATANULL));
               
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<StatusItemDTO>.CreatedFail(Status.DATANULL));

                if(checkAccount.id != checkPlan.Receiver)
                    return await Task.FromResult(PayLoad<StatusItemDTO>.CreatedFail(Status.ACCOUNTFAILD));

                var statusItemData = _mapper.Map<StatusItem>(statusItemDTO);
                statusItemData.warehousetransferstatus = checkId.id;
                statusItemData.Warehousetransferstatus_id = checkId;

                _context.statusitems.Add(statusItemData);
                _context.SaveChanges();

                if(statusItemDTO.image != null)
                {
                    var checkData = _context.statusitems.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
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
                    statusitemmap = data.id
                };

                list.Add(imageStatusItemData);
            }

            _context.imagestatusitems.AddRange(list);
            _context.SaveChanges();
        }

        public async Task<PayLoad<object>> FindByPlan(int id)
        {
            try
            {
                var checkPlan = _context.plans.Where(x => x.id == id).FirstOrDefault();
                if(checkPlan == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkStatus = _context.warehousetransferstatuses.Where(x => x.plan == checkPlan.id).FirstOrDefault();
                if(checkStatus == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(FindOneData(checkStatus)));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
