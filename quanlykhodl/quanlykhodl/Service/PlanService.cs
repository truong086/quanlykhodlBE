using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using quanlykhodl.ChatHub;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using Twilio.Rest.Trunking.V1;
using Vonage.Users;

namespace quanlykhodl.Service
{
    public class PlanService : IPlanService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserTokenAppService _userTokenAppService;
        public PlanService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService, IHubContext<NotificationHub> hubContext, IUserTokenAppService userTokenAppService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
            _hubContext = hubContext;
            _userTokenAppService = userTokenAppService;

        }
        public async Task<PayLoad<PlanDTO>> Add(PlanDTO planDTO)
        {
            try
            {
                var checkLocationExsis = _context.plans.Where(x => x.areaOld == planDTO.areaOld && x.localtionOld == planDTO.locationOld && !x.Deleted && x.status.ToLower() != Status.DONE.ToLower()).FirstOrDefault();
                if (checkLocationExsis != null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATATONTAI));
                if (!checkAddToday())
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.TODAYFULL));

                var user = _userService.name();
                
                var checkArea = _context.areas.Where(x => x.id == planDTO.areaOld && !x.Deleted).FirstOrDefault();
                if (checkArea == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.Deleted).FirstOrDefault();
                if (checkFloor == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.Deleted).FirstOrDefault();
                if (checkWarehourse == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));
                
                if(!checkQuantityLocation(planDTO))
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkAreaNew = _context.areas.Where(x => x.id == planDTO.area && !x.Deleted).FirstOrDefault();
                var chechFloorNew = _context.floors.Where(x => x.id == planDTO.floor && !x.Deleted).FirstOrDefault();
                var checkWarehourseNew = _context.warehouses.Where(x => x.id == planDTO.warehouse && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                
                var mapData = _mapper.Map<Plan>(planDTO);
                mapData.areaOld = checkArea.id;
                mapData.floorOld = checkFloor.id;
                mapData.warehouseOld = checkWarehourse.id;
                mapData.warehouse = checkWarehourseNew.id;
                mapData.warehouse_id = checkWarehourseNew;
                mapData.floor = chechFloorNew.id;
                mapData.floor_id = chechFloorNew;
                mapData.area = checkAreaNew.id;
                mapData.areaid = checkAreaNew;
                mapData.isConfirmation = false;
                mapData.isConsent = false;
                mapData.status = Status.XACNHAN;
                mapData.CretorEdit = checkAccount.username + " đã tạo Plan vào lúc " + DateTimeOffset.UtcNow;

                if (!planDTO.isWarehourse)
                {
                    var checkLocationProductOld = _context.productlocations.Where(x => x.id == planDTO.productlocation_map && !x.Deleted && x.isAction).FirstOrDefault();
                    if (checkLocationProductOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkReceiver = _context.accounts.Where(x => x.id == planDTO.Receiver && !x.Deleted).FirstOrDefault();
                    var checkProductExsis = _context.productlocations.Where(x => x.id_product == checkLocationProductOld.id_product
                                            && x.id_area == checkAreaNew.id && x.location == planDTO.localtionNew && !x.Deleted && x.isAction).FirstOrDefault();

                    if (checkLocationProductOld.location == planDTO.localtionNew &&
                    checkArea.id == checkAreaNew.id && checkFloor.id == chechFloorNew.id
                    && checkWarehourse.id == checkWarehourseNew.id)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATATONTAI));

                    if (!checkLocationQuantity(checkAreaNew, planDTO.localtionNew.Value, checkLocationProductOld.quantity))
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.FULLQUANTITY));

                    if (checkProductExsis != null)
                    {
                        checkProductExsis.quantity += checkLocationProductOld.quantity;
                        checkLocationProductOld.Deleted = true;
                        //var list = new List<productlocation>()
                        //{
                        //    checkProductExsis,
                        //    checkLocationProductOld
                        //};
                        _context.productlocations.UpdateRange(checkLocationProductOld, checkProductExsis);
                        _context.SaveChanges();

                        return await Task.FromResult(PayLoad<PlanDTO>.Successfully(planDTO));
                    }
                    
                    mapData.Receiver = checkReceiver.id;
                    mapData.Receiver_id = checkReceiver;
                    mapData.productidlocation = checkLocationProductOld;
                    mapData.productlocation_map = checkLocationProductOld.id;
                    mapData.localtionOld = checkLocationProductOld.location;
                    mapData.isWarehourse = planDTO.isWarehourse;
                }
                else
                {
                    mapData.Receiver = null;
                    mapData.Receiver_id = null;
                    mapData.localtionNew = planDTO.localtionNew;
                    mapData.localtionOld = planDTO.locationOld;
                    mapData.productidlocation = null;
                    mapData.productlocation_map = null;
                    mapData.isWarehourse = planDTO.isWarehourse;
                }

                _context.plans.Add(mapData);
                _context.SaveChanges();

                await _hubContext.Clients.All.SendAsync("thongbao", mapData.title, checkAccount.username);

                await _userTokenAppService.SendNotify();
                return await Task.FromResult(PayLoad<PlanDTO>.Successfully(planDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkQuantityLocation(PlanDTO data)
        {
            var checkLocation = _context.productlocations.Where(x => x.id_area == data.areaOld && x.location == data.locationOld && !x.Deleted).Count();
            var checkAreaNew = _context.areas.Where(x => x.id == data.area && !x.Deleted).FirstOrDefault();
            if (checkAreaNew == null)
                return false;
            var checkLocationQuantity = _context.locationExceptions.Where(x => x.id_area == checkAreaNew.id && x.location == data.localtionNew && !x.Deleted).FirstOrDefault();
            if (checkLocationQuantity != null)
            {
                if (checkLocationQuantity.max < checkLocation)
                    return false;
            }
            else
            {
                if(checkAreaNew.max < checkLocation)
                    return false;
            }
            return true;
        }

        private bool checkLocationQuantity(Area area, int location, int quantity)
        {
            var checkQuantityLocation = _context.locationExceptions.Where(x => x.id_area == area.id && x.location == location && !x.Deleted).FirstOrDefault();
            var checkTotal = _context.productlocations.Where(x => x.id_area == area.id && x.location == location && !x.Deleted && x.isAction).Sum(x => x.quantity);
            if (checkQuantityLocation != null)
            {
                if (checkQuantityLocation.max < checkTotal + quantity)
                    return false;
            }
            else
            {
                var checkArea = _context.areas.Where(x => x.id == area.id && !x.Deleted).FirstOrDefault();
                if(checkArea != null)
                {
                    if(checkArea.max < checkTotal + quantity) return false;
                }
            }

            return true;
        }

        private bool checkAddToday()
        {
            var list = new List<int>();
            // Lấy múi giờ từ Local
            TimeZoneInfo timeZone = TimeZoneInfo.Local;

            // Lấy ngày bắt đầu và ngày kết thúc (local time)
            DateTimeOffset startDate = new DateTimeOffset(DateTime.Today, timeZone.GetUtcOffset(DateTime.Now));
            DateTimeOffset endDate = startDate.AddDays(1);
            var checkPlan = _context.plans.Where(x => !x.Deleted && x.CreatedAt >= startDate && x.CreatedAt < endDate).ToList();
            if(checkPlan.Any())
            {
                foreach(var item in checkPlan)
                {
                    if(list.Count() <= 2)
                    {
                        var checkLocationProduct = _context.productlocations.Where(x => x.id == item.productlocation_map && !x.Deleted && x.isAction).FirstOrDefault();
                        if(checkLocationProduct != null)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.Deleted).FirstOrDefault();
                            if(checkProduct != null)
                                if (!list.Contains(checkProduct.category_map.Value))
                                    list.Add(checkProduct.category_map.Value);
                        }
                    }
                    else
                    {
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
                var checkId = _context.plans.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.Deleted = true;

                _context.plans.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try { 
                var data = _context.plans.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(LoadData(data), page - 1, pageSize);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<PlanGetAll> LoadData(List<Plan> data)
        {
            var list = new List<PlanGetAll>();

            if (data.Any())
            {
                foreach(var item in data)
                {
                    var mapData = _mapper.Map<PlanGetAll>(item);

                    if (!item.isWarehourse)
                    {
                        var checkProductLocation = _context.productlocations.Where(x => x.id == item.productlocation_map && !x.Deleted && x.isAction).FirstOrDefault();
                        if(checkProductLocation != null)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == checkProductLocation.id_product && !x.Deleted).FirstOrDefault();
                            if(checkProduct != null)
                            {
                                var checkImageProduct = _context.imageProducts.Where(x => x.productMap == checkProduct.id).FirstOrDefault();
                                if(checkImageProduct != null)
                                {
                                    mapData.productName = checkProduct == null ? Status.DATANULL : checkProduct.title;
                                    mapData.productImage = checkImageProduct == null ? Status.DATANULL : checkImageProduct.Link;
                                }
                                
                            }
                            
                        }
                        
                    }
                    list.Add(findOneDataMap(item, mapData));

                }
            }

            return list;
        }

        private PlanGetAll findOneDataMap(Plan item, PlanGetAll mapData)
        {
            var checkLocationNew = _context.codelocations.Where(x => x.id_area == item.area && x.location == item.localtionNew && !x.Deleted).FirstOrDefault();
            var checkCodeLocationOld = _context.codelocations.Where(x => x.id_area == item.areaOld && x.location == item.localtionOld && !x.Deleted).FirstOrDefault();

            var checkAccount = _context.accounts.Where(x => x.id == item.Receiver && !x.Deleted).FirstOrDefault();
            var checkAreaOld = _context.areas.Where(x => x.id == item.areaOld && !x.Deleted).FirstOrDefault();
            var checkFloorOld = _context.floors.Where(x => x.id == item.floorOld && !x.Deleted).FirstOrDefault();
            var checkWarehourseOld = _context.warehouses.Where(x => x.id == item.warehouseOld && !x.Deleted).FirstOrDefault();

            var checkAreaNew = _context.areas.Where(x => x.id == item.area && !x.Deleted).FirstOrDefault();
            var checkFloorNew = _context.floors.Where(x => x.id == item.floor && !x.Deleted).FirstOrDefault();
            var checkWarehourseNew = _context.warehouses.Where(x => x.id == item.warehouse && !x.Deleted).FirstOrDefault();

            mapData.localtionOldCode = checkCodeLocationOld == null ? Status.CODEFAILD : checkCodeLocationOld.code;
            mapData.localtionNewCode = checkLocationNew == null ? Status.CODEFAILD : checkLocationNew.code;
            mapData.floor = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.name;
            mapData.area = checkAreaNew == null ? Status.NOAREA : checkAreaNew.name;
            mapData.warehouse = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.name;
            mapData.floorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.name;
            mapData.areaOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.name;
            mapData.warehouseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.name;
            mapData.Receiver_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            mapData.Receiver_image = checkAccount == null ? null : checkAccount.image;
            mapData.Account_creatPlan = item.CretorEdit;
            mapData.CodeWarehourseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.code;
            mapData.CodeWarehourseNew = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.code;
            mapData.CodeFloorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.code;
            mapData.CodeFloorNew = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.code;
            mapData.CodeAreaeOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.code;
            mapData.CodeAreaeNew = checkAreaNew == null ? Status.NOFLOOR : checkAreaNew.code;

            mapData.ImageWarehourseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.image;
            mapData.ImageWarehourseNew = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.image;
            mapData.ImageFloorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.image;
            mapData.ImageFloorNew = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.image;
            mapData.ImageAreaeOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.image;
            mapData.ImageAreaeNew = checkAreaNew == null ? Status.NOFLOOR : checkAreaNew.image;

            return mapData;
        }
        public async Task<PayLoad<object>> FindConfirmationAndConsentAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.plans.Where(x => !x.Deleted && x.isConfirmation == true && x.isConsent == true).ToList();

                if(!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

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

        public async Task<PayLoad<object>> FindConfirmationAndConsentByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                var checkPlan = _context.plans.Where(x => x.Receiver == checkAccount.id && !x.Deleted && x.isConfirmation == true && x.isConsent == true).ToList();

                if (!string.IsNullOrEmpty(name))
                    checkPlan = checkPlan.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(checkPlan, page - 1, pageSize);

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

        public async Task<PayLoad<object>> FindConfirmationAndNoConsentAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.plans.Where(x => x.isConfirmation == true && x.isConsent == false && !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

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

        public async Task<PayLoad<object>> FindConfirmationAndNoConsentByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkPlan = _context.plans.Where(x => x.Receiver == Convert.ToInt32(user) 
                && !x.Deleted && x.isConsent == false 
                && x.isConfirmation == true).ToList();

                if (!string.IsNullOrEmpty(name))
                    checkPlan = checkPlan.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(LoadData(checkPlan), page - 1, pageSize);

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

        public async Task<PayLoad<object>> FindNoConfirmationAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.plans.Where(x => x.isConfirmation == false && x.isConsent == false && !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name)).ToList();

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

        public async Task<PayLoad<object>> FindNoConfirmationByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkPlan = _context.plans.Where(x => x.isConsent == false && x.isConfirmation == false 
                && !x.Deleted && x.Receiver == int.Parse(user)).ToList();

                if (!string.IsNullOrEmpty(name))
                    checkPlan = checkPlan.Where(x => x.title.Contains(name)).ToList();

                var pageList = new PageList<object>(LoadData(checkPlan), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));

            }catch(Exception ex )
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<PlanGetAll>> FindOne(int id)
        {
            try
            {
                var checkId = _context.plans.Where(x => x.id == id).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<PlanGetAll>.CreatedFail(Status.DATANULL));
                var mapData = _mapper.Map<PlanGetAll>(checkId);
                return await Task.FromResult(PayLoad<PlanGetAll>.Successfully(findOneDataMap(checkId, mapData)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<PlanGetAll>.CreatedFail(ex.Message));
            }
        }

        private PlanGetAll loadDataFindOne(Plan item)
        {
            //var checkProductLocation = _context.productlocations.Where(x => x.id == item.productlocation_map && !x.Deleted && x.isAction).FirstOrDefault();
            //var checkProduct = _context.products1.Where(x => x.id == checkProductLocation.id_product && !x.Deleted).FirstOrDefault();
            //var checkImageProduct = _context.imageProducts.Where(x => x.productMap == checkProduct.id).FirstOrDefault();
            var checkAccount = _context.accounts.Where(x => x.id == item.Receiver && !x.Deleted).FirstOrDefault();
            var checkAreaOld = _context.areas.Where(x => x.id == item.areaOld && !x.Deleted).FirstOrDefault();
            var checkFloorOld = _context.floors.Where(x => x.id == item.floorOld && !x.Deleted).FirstOrDefault();
            var checkWarehourseOld = _context.warehouses.Where(x => x.id == item.warehouseOld && !x.Deleted).FirstOrDefault();

            var checkAreaNew = _context.areas.Where(x => x.id == item.area && !x.Deleted).FirstOrDefault();
            var checkFloorNew = _context.floors.Where(x => x.id == item.floor && !x.Deleted).FirstOrDefault();
            var checkWarehourseNew = _context.warehouses.Where(x => x.id == item.warehouse && !x.Deleted).FirstOrDefault();

            var mapData = _mapper.Map<PlanGetAll>(item);
            mapData.floor = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.name;
            mapData.area = checkAreaNew == null ? Status.NOAREA : checkAreaNew.name;
            mapData.warehouse = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.name;
            mapData.floorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.name;
            mapData.areaOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.name;
            mapData.warehouseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.name;
            mapData.Receiver_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            //mapData.productName = checkProduct == null ? Status.DATANULL : checkProduct.title;
            //mapData.productImage = checkImageProduct == null ? Status.DATANULL : checkImageProduct.Link;
            mapData.Account_creatPlan = item.CretorEdit;

            return mapData;
        }

        public async Task<PayLoad<PlanDTO>> Update(int id, PlanDTO planDTO)
        {
            try
            {
                var user = _userService.name();

                var checkId = _context.plans.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if(checkId == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkAreaNew = _context.areas.Where(x => x.id == planDTO.area && !x.Deleted).FirstOrDefault();
                var chechFloorNew = _context.floors.Where(x => x.id == planDTO.floor && !x.Deleted).FirstOrDefault();
                var checkWarehourseNew = _context.warehouses.Where(x => x.id == planDTO.warehouse && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();

                if(planDTO.Receiver != 0 && planDTO.Receiver != null)
                {
                    if (checkId.Receiver != planDTO.Receiver)
                    {
                        var checkAccountUpdate = _context.accounts.Where(x => x.id == planDTO.Receiver && !x.Deleted).FirstOrDefault();
                        checkId.Receiver = checkAccountUpdate.id;
                        checkId.Receiver_id = checkAccountUpdate;

                        var checkStatusPlanWarehourse = _context.warehousetransferstatuses.Where(x => x.plan == checkId.id && !x.Deleted).FirstOrDefault();
                        if (checkStatusPlanWarehourse != null)
                        {
                            checkStatusPlanWarehourse.Deleted = true;
                            _context.warehousetransferstatuses.Update(checkStatusPlanWarehourse);
                            _context.SaveChanges();
                        }
                    }
                }
                var mapDataUpdate = MapperData.GanData(checkId, planDTO);
                mapDataUpdate.area = checkAreaNew.id;
                mapDataUpdate.areaid = checkAreaNew;
                mapDataUpdate.floor = chechFloorNew.id;
                mapDataUpdate.floor_id = chechFloorNew;
                mapDataUpdate.warehouse = checkWarehourseNew.id;
                mapDataUpdate.warehouse_id = checkWarehourseNew;

                if (!planDTO.isWarehourse)
                {
                    if (checkId.productlocation_map != planDTO.productlocation_map)
                    {
                        var checkLocationProductOld = _context.productlocations.Where(x => x.id == planDTO.productlocation_map && !x.Deleted && x.isAction).FirstOrDefault();
                        if (checkLocationProductOld == null)
                            return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                        mapDataUpdate.productlocation_map = checkLocationProductOld.id;
                        mapDataUpdate.productidlocation = checkLocationProductOld;
                        mapDataUpdate.localtionOld = checkLocationProductOld.location;
                    }
                }
                else
                {
                    var checkAreaOld = _context.areas.Where(x => x.id == planDTO.areaOld && !x.Deleted).FirstOrDefault();
                    if (checkAreaOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkFloorOld = _context.floors.Where(x => x.id == checkAreaOld.floor && !x.Deleted).FirstOrDefault();
                    if (checkFloorOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkWarehourseOla = _context.warehouses.Where(x => x.id == checkFloorOld.warehouse && !x.Deleted).FirstOrDefault();
                    if (checkWarehourseOla == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));
                  
                    mapDataUpdate.areaOld = checkAreaOld.id;
                    mapDataUpdate.floorOld = checkFloorOld.id;
                    mapDataUpdate.warehouseOld = checkWarehourseOla.id;
                    mapDataUpdate.localtionNew = null;
                    mapDataUpdate.localtionOld = null;
                    mapDataUpdate.productidlocation = null;
                    mapDataUpdate.productlocation_map = null;
                }
                
                _context.plans.Update(mapDataUpdate);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<PlanDTO>.Successfully(planDTO));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<bool>> UpdatePlanConfirmation(ConfirmationPlan data)
        {
            try
            {
                if(data.id != null && data.id.Any())
                {
                    var user = _userService.name();
                    var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();
                    foreach(var item in data.id)
                    {
                        if(item <= 0)
                            return await Task.FromResult(PayLoad<bool>.Successfully(false));
                        
                        var checkId = _context.plans.Where(x => x.id == item && !x.isConfirmation && !x.isConsent && !x.Deleted).FirstOrDefault();
                        if (checkId == null)
                            return await Task.FromResult(PayLoad<bool>.Successfully(false));
                        
                        if (data.isConfirmation == true)
                        {
                            checkId.isConfirmation = true;
                            checkId.isConsent = true;
                            checkId.status = Status.DANHAN.ToLower();
                            checkId.Receiver = checkAccount == null ? null : checkAccount.id;
                            checkId.Receiver_id = checkAccount == null ? null : checkAccount;
                            var warehourseStarus = new Warehousetransferstatus
                            {
                                plan_id = checkId,
                                plan = checkId.id,
                                status = Status.DANHAN.ToLower(),
                                Deleted = false
                            };

                            checkId.UpdatedAt = DateTimeOffset.UtcNow;

                            _context.warehousetransferstatuses.Add(warehourseStarus);
                            _context.plans.Update(checkId);
                            _context.SaveChanges();
                            await _hubContext.Clients.All.SendAsync("confirm", checkAccount.username, checkId.title);
                        }
                    }
                }
                return await Task.FromResult(PayLoad<bool>.Successfully(true));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<bool>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllByAccountPlanNoConfirmByDate()
        {
            try
            {
                var user = _userService.name();
                // Lấy múi giờ từ Local
                TimeZoneInfo timeZone = TimeZoneInfo.Local;

                // Lấy ngày bắt đầu và ngày kết thúc (local time)
                DateTimeOffset startDate = new DateTimeOffset(DateTime.Today, timeZone.GetUtcOffset(DateTime.Now));
                DateTimeOffset endDate = startDate.AddDays(1);

                // Lọc những bản ghi trong khoảng thời gian
                var checkData = _context.plans.Where(x => x.Receiver == int.Parse(user) && x.CreatedAt >= startDate && x.CreatedAt < endDate && !x.Deleted).Count();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = checkData
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<PlanAllWarehoursDTO>> AddAllWarehours(PlanAllWarehoursDTO planDTO)
        {
            try
            {
                var checkWarehoure = _context.warehouses.Where(x => x.id == planDTO.warehouse && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == planDTO.floor && !x.Deleted).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.id == planDTO.area && !x.Deleted).FirstOrDefault();
                if (checkWarehoure == null || checkFloor == null || checkArea == null)
                    return await Task.FromResult(PayLoad<PlanAllWarehoursDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Plan>(planDTO);

                return await Task.FromResult(PayLoad<PlanAllWarehoursDTO>.Successfully(planDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<PlanAllWarehoursDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindDoneByAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkDone = _context.plans.Where(x => x.status.ToLower() == Status.DONE.ToLower()).ToList();
                if(checkDone.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                if (!string.IsNullOrEmpty(name))
                    checkDone = checkDone.Where(x => x.title.Contains(name)).ToList();
                var pageList = new PageList<object>(LoadData(checkDone), page - 1, pageSize);

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

        public async Task<PayLoad<object>> FindDoneByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();
                if(checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkDone = _context.plans.Where(x => x.status.ToLower() == Status.DONE.ToLower() && x.Receiver == checkAccount.id && x.isConfirmation).ToList();
                if (checkDone.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                if (!string.IsNullOrEmpty(name))
                    checkDone = checkDone.Where(x => x.title.Contains(name)).ToList();
                var pageList = new PageList<object>(LoadData(checkDone), page - 1, pageSize);

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

        public async Task<PayLoad<object>> FindConfirmationByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkDone = _context.plans.Where(x => x.status.ToLower() != Status.DONE.ToLower() && x.Receiver == checkAccount.id && x.isConfirmation && !x.Deleted).ToList();
                if (checkDone.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                if (!string.IsNullOrEmpty(name))
                    checkDone = checkDone.Where(x => x.title.Contains(name)).ToList();
                var pageList = new PageList<object>(LoadData(checkDone), page - 1, pageSize);

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
    }
}
