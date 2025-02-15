using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
                var checkLcoation = _context.productlocations.Where(x => x.id_shelf == planDTO.shelfOld && x.location == planDTO.locationOld && !x.deleted && x.isaction).FirstOrDefault();
                if(checkLcoation == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkLocationExsis = _context.plans.Where(x => ((x.shelfOld == planDTO.shelfOld && x.localtionold == planDTO.locationOld) || (x.shelf == planDTO.shelf && x.localtionnew == planDTO.localtionNew)) && !x.deleted && x.status.ToLower() != Status.DONE.ToLower()).FirstOrDefault();
                if (checkLocationExsis != null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATATONTAIPLAN));
                if (!checkAddToday())
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.TODAYFULL));

                var user = _userService.name();
                
                var checkShelf = _context.shelfs.Where(x => x.id == planDTO.shelfOld && !x.deleted).FirstOrDefault();
                if (checkShelf == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                if (checkArea == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                if (checkFloor == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                if (checkWarehourse == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));
                
                if(!checkQuantityLocation(planDTO))
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkShelfNew = _context.shelfs.Where(x => x.id == planDTO.shelf && !x.deleted).FirstOrDefault();
                var checkAreaNew = _context.areas.Where(x => x.id == planDTO.area && !x.deleted).FirstOrDefault();
                var chechFloorNew = _context.floors.Where(x => x.id == planDTO.floor && !x.deleted).FirstOrDefault();
                var checkWarehourseNew = _context.warehouses.Where(x => x.id == planDTO.warehouse && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                
                var mapData = _mapper.Map<Plan>(planDTO);
                mapData.shelfOld = checkShelf.id;
                mapData.areaold = checkArea.id;
                mapData.floorold = checkFloor.id;
                mapData.warehouseold = checkWarehourse.id;
                mapData.warehouse = checkWarehourseNew.id;
                mapData.warehouse_id = checkWarehourseNew;
                mapData.floor = chechFloorNew.id;
                mapData.floor_id = chechFloorNew;
                mapData.area = checkAreaNew.id;
                mapData.area_id = checkAreaNew;
                mapData.shelf = checkShelfNew.id;
                mapData.shelfid = checkShelfNew;
                mapData.isconfirmation = false;
                mapData.isconsent = false;
                mapData.status = Status.XACNHAN;
                mapData.cretoredit = checkAccount.username + " đã tạo Plan vào lúc " + DateTimeOffset.UtcNow;

                if (!planDTO.isWarehourse)
                {
                    var checkLocationProductOld = _context.productlocations.Where(x => x.id == planDTO.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
                    if (checkLocationProductOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkReceiver = _context.accounts.Where(x => x.id == planDTO.Receiver && !x.deleted).FirstOrDefault();
                    var checkProductExsis = _context.productlocations.Where(x => x.id_product == checkLocationProductOld.id_product
                                            && x.id_shelf == checkAreaNew.id && x.location == planDTO.localtionNew && !x.deleted && x.isaction).FirstOrDefault();

                    if (checkLocationProductOld.location == planDTO.localtionNew &&
                    checkArea.id == checkAreaNew.id && checkFloor.id == chechFloorNew.id
                    && checkWarehourse.id == checkWarehourseNew.id)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATATONTAI));

                    if (!checkLocationQuantity(checkShelfNew, planDTO.localtionNew.Value, checkLocationProductOld.quantity))
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.FULLQUANTITY));

                    if (checkProductExsis != null)
                    {
                        checkProductExsis.quantity += checkLocationProductOld.quantity;
                        checkLocationProductOld.deleted = true;
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
                    mapData.localtionold = checkLocationProductOld.location;
                    mapData.iswarehourse = planDTO.isWarehourse;
                }
                else
                {
                    mapData.Receiver = null;
                    mapData.Receiver_id = null;
                    mapData.localtionnew = planDTO.localtionNew;
                    mapData.localtionold = planDTO.locationOld;
                    mapData.productidlocation = null;
                    mapData.productlocation_map = null;
                    mapData.iswarehourse = planDTO.isWarehourse;
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
            var checkLocation = _context.productlocations.Where(x => x.id_shelf == data.shelfOld && x.location == data.locationOld && !x.deleted).Count();
            var checkAreaNew = _context.shelfs.Where(x => x.id == data.shelf && !x.deleted).FirstOrDefault();
            if (checkAreaNew == null)
                return false;
            var checkLocationQuantity = _context.locationexceptions.Where(x => x.id_shelf == checkAreaNew.id && x.location == data.localtionNew && !x.deleted).FirstOrDefault();
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

        private bool checkLocationQuantity(Shelf shelf, int location, int quantity)
        {
            var checkQuantityLocation = _context.locationexceptions.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted).FirstOrDefault();
            var checkTotal = _context.productlocations.Where(x => x.id_shelf == shelf.id && x.location == location && !x.deleted && x.isaction).Sum(x => x.quantity);
            if (checkQuantityLocation != null)
            {
                if (checkQuantityLocation.max < checkTotal + quantity)
                    return false;
            }
            else
            {
                var checkShelf = _context.shelfs.Where(x => x.id == shelf.id && !x.deleted).FirstOrDefault();
                if(checkShelf != null)
                {
                    if(checkShelf.max < checkTotal + quantity) return false;
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
            var checkPlan = _context.plans.Where(x => !x.deleted && x.createdat >= startDate && x.createdat < endDate).ToList();
            if(checkPlan.Any())
            {
                foreach(var item in checkPlan)
                {
                    if(list.Count() <= 2)
                    {
                        var checkLocationProduct = _context.productlocations.Where(x => x.id == item.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
                        if(checkLocationProduct != null)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == checkLocationProduct.id_product && !x.deleted).FirstOrDefault();
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
                var checkId = _context.plans.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;

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
                var data = _context.plans.Where(x => !x.deleted).ToList();

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

                    if (!item.iswarehourse)
                    {
                        var checkProductLocation = _context.productlocations.Where(x => x.id == item.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
                        if(checkProductLocation != null)
                        {
                            var checkProduct = _context.products1.Where(x => x.id == checkProductLocation.id_product && !x.deleted).FirstOrDefault();
                            if(checkProduct != null)
                            {
                                var checkImageProduct = _context.imageproducts.Where(x => x.productmap == checkProduct.id).FirstOrDefault();
                                if(checkImageProduct != null)
                                {
                                    mapData.productName = checkProduct == null ? Status.DATANULL : checkProduct.title;
                                    mapData.productImage = checkImageProduct == null ? Status.DATANULL : checkImageProduct.link;
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
            var checkLocationNew = _context.codelocations.Where(x => x.id_helf == item.shelf && x.location == item.localtionnew && !x.deleted).FirstOrDefault();
            var checkCodeLocationOld = _context.codelocations.Where(x => x.id_helf == item.shelfOld && x.location == item.localtionold && !x.deleted).FirstOrDefault();

            var checkAccount = _context.accounts.Where(x => x.id == item.Receiver && !x.deleted).FirstOrDefault();
            var checkShelfOld = _context.shelfs.Where(x => x.id == item.shelfOld && !x.deleted).FirstOrDefault();
            var checkAreaOld = _context.areas.Where(x => x.id == item.areaold && !x.deleted).FirstOrDefault();
            var checkFloorOld = _context.floors.Where(x => x.id == item.floorold && !x.deleted).FirstOrDefault();
            var checkWarehourseOld = _context.warehouses.Where(x => x.id == item.warehouseold && !x.deleted).FirstOrDefault();

            var checkShelfNew = _context.shelfs.Where(x => x.id == item.shelf && !x.deleted).FirstOrDefault();
            var checkAreaNew = _context.areas.Where(x => x.id == item.area && !x.deleted).FirstOrDefault();
            var checkFloorNew = _context.floors.Where(x => x.id == item.floor && !x.deleted).FirstOrDefault();
            var checkWarehourseNew = _context.warehouses.Where(x => x.id == item.warehouse && !x.deleted).FirstOrDefault();

            mapData.localtionOldCode = checkCodeLocationOld == null ? Status.CODEFAILD : checkCodeLocationOld.code;
            mapData.localtionNewCode = checkLocationNew == null ? Status.CODEFAILD : checkLocationNew.code;
            mapData.floor = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.name;
            mapData.area = checkAreaNew == null ? Status.NOAREA : checkAreaNew.name;
            mapData.shelf = checkShelfNew == null ? Status.NOAREA : checkShelfNew.name;
            mapData.warehouse = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.name;
            mapData.floorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.name;
            mapData.areaOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.name;
            mapData.warehouseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.name;
            mapData.shelfOld = checkShelfOld == null ? Status.NOWAREHOURSE : checkShelfOld.name;
            mapData.Receiver_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            mapData.Receiver_image = checkAccount == null ? null : checkAccount.image;
            mapData.Account_creatPlan = item.cretoredit;
            mapData.CodeWarehourseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.code;
            mapData.CodeWarehourseNew = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.code;
            mapData.CodeFloorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.code;
            mapData.CodeFloorNew = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.code;
            mapData.CodeAreaeOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.code;
            mapData.CodeAreaeNew = checkAreaNew == null ? Status.NOFLOOR : checkAreaNew.code;
            mapData.CodeShelfNew = checkShelfNew == null ? Status.NOFLOOR : checkShelfNew.code;
            mapData.CodeShelfOld = checkShelfOld == null ? Status.NOFLOOR : checkShelfOld.code;

            mapData.ImageWarehourseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.image;
            mapData.ImageWarehourseNew = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.image;
            mapData.ImageFloorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.image;
            mapData.ImageFloorNew = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.image;
            mapData.ImageAreaeOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.image;
            mapData.ImageAreaeNew = checkAreaNew == null ? Status.NOFLOOR : checkAreaNew.image;
            mapData.ImageShelfeNew = checkShelfNew == null ? Status.NOFLOOR : checkShelfNew.image;
            mapData.ImageShelfeOld = checkShelfOld == null ? Status.NOFLOOR : checkShelfOld.image;

            mapData.idWarehouseNew = checkWarehourseNew == null ? null : checkWarehourseNew.id;
            mapData.idFloorNew = checkFloorNew == null ? null : checkFloorNew.id;
            mapData.idAreaNew = checkAreaNew == null ? null : checkAreaNew.id;
            mapData.idShelfNew = checkShelfNew == null ? null : checkShelfNew.id;
            mapData.idShelfOld = checkShelfOld == null ? null : checkShelfOld.id;

            return mapData;
        }
        public async Task<PayLoad<object>> FindConfirmationAndConsentAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.plans.Where(x => !x.deleted && x.isconfirmation == true && x.isconsent == true).ToList();

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
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                var checkPlan = _context.plans.Where(x => x.Receiver == checkAccount.id && !x.deleted && x.isconfirmation == true && x.isconsent == true).ToList();

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
                var data = _context.plans.Where(x => x.isconfirmation == true && x.isconsent == false && !x.deleted).ToList();

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
                && !x.deleted && x.isconsent == false 
                && x.isconfirmation == true).ToList();

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
                var data = _context.plans.Where(x => x.isconfirmation == false && x.isconsent == false && !x.deleted).ToList();

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
                var checkPlan = _context.plans.Where(x => x.isconsent == false && x.isconfirmation == false 
                && !x.deleted && x.Receiver == int.Parse(user)).ToList();

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
            //var checkProductLocation = _context.productlocations.Where(x => x.id == item.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
            //var checkProduct = _context.products1.Where(x => x.id == checkProductLocation.id_product && !x.deleted).FirstOrDefault();
            //var checkImageProduct = _context.imageproducts.Where(x => x.productmap == checkProduct.id).FirstOrDefault();
            var checkAccount = _context.accounts.Where(x => x.id == item.Receiver && !x.deleted).FirstOrDefault();
            var checkAreaOld = _context.shelfs.Where(x => x.id == item.shelfOld && !x.deleted).FirstOrDefault();
            var checkFloorOld = _context.floors.Where(x => x.id == item.floorold && !x.deleted).FirstOrDefault();
            var checkWarehourseOld = _context.warehouses.Where(x => x.id == item.warehouseold && !x.deleted).FirstOrDefault();

            var checkAreaNew = _context.shelfs.Where(x => x.id == item.shelf && !x.deleted).FirstOrDefault();
            var checkFloorNew = _context.floors.Where(x => x.id == item.floor && !x.deleted).FirstOrDefault();
            var checkWarehourseNew = _context.warehouses.Where(x => x.id == item.warehouse && !x.deleted).FirstOrDefault();

            var mapData = _mapper.Map<PlanGetAll>(item);
            mapData.floor = checkFloorNew == null ? Status.NOFLOOR : checkFloorNew.name;
            mapData.area = checkAreaNew == null ? Status.NOAREA : checkAreaNew.name;
            mapData.warehouse = checkWarehourseNew == null ? Status.NOWAREHOURSE : checkWarehourseNew.name;
            mapData.floorOld = checkFloorOld == null ? Status.NOFLOOR : checkFloorOld.name;
            mapData.areaOld = checkAreaOld == null ? Status.NOAREA : checkAreaOld.name;
            mapData.warehouseOld = checkWarehourseOld == null ? Status.NOWAREHOURSE : checkWarehourseOld.name;
            mapData.Receiver_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
            //mapData.productName = checkProduct == null ? status.DATANULL : checkProduct.title;
            //mapData.productImage = checkImageProduct == null ? status.DATANULL : checkImageProduct.link;
            mapData.Account_creatPlan = item.cretoredit;

            return mapData;
        }

        public async Task<PayLoad<PlanDTO>> Update(int id, PlanDTO planDTO)
        {
            try
            {
                var user = _userService.name();

                var checkId = _context.plans.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkLcoation = _context.productlocations.Where(x => x.id_shelf == planDTO.shelfOld && x.location == planDTO.locationOld && !x.deleted && x.isaction).FirstOrDefault();
                if (checkLcoation == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkLocationExsis = _context.plans.Where(x => ((x.shelfOld == planDTO.shelfOld && x.localtionold == planDTO.locationOld) || (x.shelf == planDTO.shelf && x.localtionnew == planDTO.localtionNew)) && x.id != checkId.id && !x.deleted && x.status.ToLower() != Status.DONE.ToLower()).FirstOrDefault();
                if (checkLocationExsis != null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATATONTAIPLAN));
                if (!checkAddToday())
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.TODAYFULL));

                var checkShelf = _context.shelfs.Where(x => x.id == planDTO.shelfOld && !x.deleted).FirstOrDefault();
                if (checkShelf == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkArea = _context.areas.Where(x => x.id == checkShelf.line && !x.deleted).FirstOrDefault();
                if (checkArea == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkFloor = _context.floors.Where(x => x.id == checkArea.floor && !x.deleted).FirstOrDefault();
                if (checkFloor == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                var checkWarehourse = _context.warehouses.Where(x => x.id == checkFloor.warehouse && !x.deleted).FirstOrDefault();
                if (checkWarehourse == null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                if (!checkQuantityLocation(planDTO))
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkLocationExsiss = _context.plans.Where(x => (x.shelfOld == planDTO.shelfOld && x.localtionold == planDTO.locationOld) 
                && (x.shelfOld != planDTO.shelfOld && x.localtionold != planDTO.locationOld) 
                && x.id != checkId.id
                && !x.deleted && x.status.ToLower() != Status.DONE.ToLower())
                    .FirstOrDefault();

                if (checkLocationExsiss != null)
                    return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATATONTAIPLAN));
                

                var checkShelfNew = _context.shelfs.Where(x => x.id == planDTO.shelf && !x.deleted).FirstOrDefault();
                var checkAreaNew = _context.areas.Where(x => x.id == planDTO.area && !x.deleted).FirstOrDefault();
                var chechFloorNew = _context.floors.Where(x => x.id == planDTO.floor && !x.deleted).FirstOrDefault();
                var checkWarehourseNew = _context.warehouses.Where(x => x.id == planDTO.warehouse && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();

                if(planDTO.Receiver != 0 && planDTO.Receiver != null)
                {
                    if (checkId.Receiver != planDTO.Receiver)
                    {
                        var checkAccountUpdate = _context.accounts.Where(x => x.id == planDTO.Receiver && !x.deleted).FirstOrDefault();
                        checkId.Receiver = checkAccountUpdate.id;
                        checkId.Receiver_id = checkAccountUpdate;

                        var checkStatusPlanWarehourse = _context.warehousetransferstatuses.Where(x => x.plan == checkId.id && !x.deleted).FirstOrDefault();
                        if (checkStatusPlanWarehourse != null)
                        {
                            checkStatusPlanWarehourse.deleted = true;
                            _context.warehousetransferstatuses.Update(checkStatusPlanWarehourse);
                            _context.SaveChanges();
                        }
                    }
                }
                var mapDataUpdate = MapperData.GanData(checkId, planDTO);
                mapDataUpdate.shelf = checkShelfNew.id;
                mapDataUpdate.shelfid = checkShelfNew;
                mapDataUpdate.area_id = checkAreaNew;
                mapDataUpdate.area = checkAreaNew.id;
                mapDataUpdate.floor = chechFloorNew.id;
                mapDataUpdate.floor_id = chechFloorNew;
                mapDataUpdate.warehouse = checkWarehourseNew.id;
                mapDataUpdate.warehouse_id = checkWarehourseNew;

                if (!planDTO.isWarehourse)
                {
                    if (checkId.productlocation_map != planDTO.productlocation_map)
                    {
                        var checkLocationProductOld = _context.productlocations.Where(x => x.id == planDTO.productlocation_map && !x.deleted && x.isaction).FirstOrDefault();
                        if (checkLocationProductOld == null)
                            return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                        mapDataUpdate.productlocation_map = checkLocationProductOld.id;
                        mapDataUpdate.productidlocation = checkLocationProductOld;
                        mapDataUpdate.localtionold = checkLocationProductOld.location;
                    }
                }
                else
                {
                    var checkShelfOld = _context.shelfs.Where(x => x.id == planDTO.shelfOld && !x.deleted).FirstOrDefault();
                    if(checkShelfOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkAreaOld = _context.areas.Where(x => x.id == checkShelfOld.line && !x.deleted).FirstOrDefault();
                    if (checkAreaOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkFloorOld = _context.floors.Where(x => x.id == checkAreaOld.floor && !x.deleted).FirstOrDefault();
                    if (checkFloorOld == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));

                    var checkWarehourseOla = _context.warehouses.Where(x => x.id == checkFloorOld.warehouse && !x.deleted).FirstOrDefault();
                    if (checkWarehourseOla == null)
                        return await Task.FromResult(PayLoad<PlanDTO>.CreatedFail(Status.DATANULL));
                  
                    mapDataUpdate.shelfOld = checkShelfOld.id;
                    mapDataUpdate.areaold = checkAreaOld.id;
                    mapDataUpdate.floorold = checkFloorOld.id;
                    mapDataUpdate.warehouseold = checkWarehourseOla.id;
                    mapDataUpdate.localtionnew = planDTO.localtionNew;
                    mapDataUpdate.localtionold = planDTO.locationOld;
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
                    var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                    foreach(var item in data.id)
                    {
                        if(item <= 0)
                            return await Task.FromResult(PayLoad<bool>.Successfully(false));
                        
                        var checkId = _context.plans.Where(x => x.id == item && !x.isconfirmation && !x.isconsent && !x.deleted).FirstOrDefault();
                        if (checkId == null)
                            return await Task.FromResult(PayLoad<bool>.Successfully(false));
                        
                        if (data.isConfirmation == true)
                        {
                            checkId.isconfirmation = true;
                            checkId.isconsent = true;
                            checkId.status = Status.DANHAN.ToLower();
                            checkId.Receiver = checkAccount == null ? null : checkAccount.id;
                            checkId.Receiver_id = checkAccount == null ? null : checkAccount;
                            var warehourseStarus = new Warehousetransferstatus
                            {
                                plan_id = checkId,
                                plan = checkId.id,
                                status = Status.DANHAN.ToLower(),
                                deleted = false
                            };

                            checkId.updatedat = DateTimeOffset.UtcNow;

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
                var checkData = _context.plans.Where(x => x.Receiver == int.Parse(user) && x.createdat >= startDate && x.createdat < endDate && !x.deleted).Count();

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
                var checkWarehoure = _context.warehouses.Where(x => x.id == planDTO.warehouse && !x.deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == planDTO.floor && !x.deleted).FirstOrDefault();
                var checkArea = _context.shelfs.Where(x => x.id == planDTO.area && !x.deleted).FirstOrDefault();
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
                var checkDone = _context.plans.Where(x => x.status.ToLower() == Status.DONE.ToLower()).OrderByDescending(x => x.updatedat).ToList();
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
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if(checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkDone = _context.plans.Where(x => x.status.ToLower() == Status.DONE.ToLower() && x.Receiver == checkAccount.id && x.isconfirmation).OrderByDescending(x => x.updatedat).ToList();
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
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkDone = _context.plans.Where(x => x.status.ToLower() != Status.DONE.ToLower() && x.Receiver == checkAccount.id && x.isconfirmation && !x.deleted).ToList();
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

        public async Task<PayLoad<object>> FindAllDataByDate(searchDatetimePlan datetimePlan, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.plans.Where(x => x.isconfirmation && x.status.ToLower() == Status.DONE.ToLower() && x.updatedat >= datetimePlan.datefrom && x.updatedat <= datetimePlan.dateto).ToList();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var pageList = new PageList<object>(LoadData(checkData), page - 1, pageSize);

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

        public byte[] FindAllDataByDateExcel(searchDatetimePlan datetimePlan)
        {
            var checkData = _context.plans.Where(x => x.isconfirmation && x.status.ToLower() == Status.DONE.ToLower() && x.updatedat >= datetimePlan.datefrom && x.updatedat <= datetimePlan.dateto).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Title";
                worksheet.Cells[1, 3].Value = "status";
                worksheet.Cells[1, 4].Value = "localtionNew";
                worksheet.Cells[1, 5].Value = "isConfirmation";
                worksheet.Cells[1, 6].Value = "receiver_name";
                worksheet.Cells[1, 7].Value = "localtionOld";
                worksheet.Cells[1, 8].Value = "localtionOldCode";
                worksheet.Cells[1, 9].Value = "localtionNewCode";
                worksheet.Cells[1, 10].Value = "warehouseOld";
                worksheet.Cells[1, 11].Value = "areaOld";
                worksheet.Cells[1, 12].Value = "shelfOld";
                worksheet.Cells[1, 13].Value = "floorOld";
                worksheet.Cells[1, 14].Value = "warehouse";
                worksheet.Cells[1, 15].Value = "area";
                worksheet.Cells[1, 16].Value = "shelf";
                worksheet.Cells[1, 17].Value = "floor";
                worksheet.Cells[1, 18].Value = "account_creatPlan";
                worksheet.Cells[1, 19].Value = "updatedAt";
                worksheet.Cells[1, 20].Value = "codeWarehourseNew";

                // Định dạng tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 20])
                {
                    range.Style.Font.Bold = true; // Chữ in đậm
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid; // Nền đặc
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Nền xám nhạt
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa nội dung
                }

                // Đổ dữ liệu vào file Excel
                int row = 2;
                foreach (var product in LoadData(checkData))
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.title;
                    worksheet.Cells[row, 3].Value = product.status;
                    worksheet.Cells[row, 4].Value = product.localtionNew;
                    worksheet.Cells[row, 5].Value = product.isConfirmation;
                    worksheet.Cells[row, 6].Value = product.Receiver_name;
                    worksheet.Cells[row, 7].Value = product.localtionOld;
                    worksheet.Cells[row, 8].Value = product.localtionOldCode;
                    worksheet.Cells[row, 9].Value = product.localtionNewCode;
                    worksheet.Cells[row, 10].Value = product.warehouseOld;
                    worksheet.Cells[row, 11].Value = product.areaOld;
                    worksheet.Cells[row, 12].Value = product.shelfOld;
                    worksheet.Cells[row, 13].Value = product.floorOld;
                    worksheet.Cells[row, 14].Value = product.warehouse;
                    worksheet.Cells[row, 15].Value = product.area;
                    worksheet.Cells[row, 16].Value = product.shelf;
                    worksheet.Cells[row, 17].Value = product.floor;
                    worksheet.Cells[row, 18].Value = product.Account_creatPlan;
                    worksheet.Cells[row, 19].Value = product.UpdatedAt;
                    worksheet.Cells[row, 20].Value = product.CodeWarehourseNew;
                    row++;
                }

                worksheet.Cells.AutoFitColumns(); // Tự động chỉnh độ rộng cột
                return package.GetAsByteArray();
            }
        }

        public List<PlanGetAll> FindALlDataExcel(searchDatetimePlan datetimePlan)
        {
            var checkData = _context.plans.Where(x => x.isconfirmation && x.status.ToLower() == Status.DONE.ToLower() && x.updatedat >= datetimePlan.datefrom && x.updatedat <= datetimePlan.dateto).ToList();
            return LoadData(checkData);
        }
    }
}
