using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using Vonage.Users;

namespace quanlykhodl.Service
{
    public class FloorService : IFloorService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public FloorService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<FloorDTO>> Add(FloorDTO floorDTO)
        {
            try
            {
                var user = _userService.name();
                if (!checkFullQuantity(floorDTO.warehouse))
                    return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkWarehouse = findOneWarehouseDTO(floorDTO.warehouse);

                var checkName = _context.floors.Where(x => x.name == floorDTO.name && !x.Deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(Status.DATANULL));

                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<Floor>(floorDTO);
                mapData.warehouse_id = checkWarehouse;
                mapData.warehouse = checkWarehouse.id;
                mapData.accounts = checkAccount;
                mapData.account_id = checkAccount.id;
                mapData.code = RanDomCode.geneAction(8);

                _context.floors.Add(mapData);
                _context.SaveChanges();

                if(floorDTO.image != null)
                {
                    var dataNew = _context.floors.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    uploadCloud.CloudInaryIFromAccount(floorDTO.image, TokenViewModel.FLOOR + dataNew.id.ToString(), _cloud);
                    dataNew.image = uploadCloud.Link;
                    dataNew.publicid = uploadCloud.publicId;

                    _context.floors.Update(dataNew);
                    _context.SaveChanges();
                }

                return await Task.FromResult(PayLoad<FloorDTO>.Successfully(floorDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(ex.Message));
            }
        }

        private Warehouse findOneWarehouseDTO(int? id)
        {
            var checkWarehouse = _context.warehouses.Where(x => x.id == id && !x.Deleted).FirstOrDefault();

            return checkWarehouse == null ? null : checkWarehouse;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkData = _context.floors.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                checkData.Deleted = true;

                _context.floors.Update(checkData);

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
                var data = _context.floors.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

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

        private List<FloorGetAll> LoadData(List<Floor> data)
        {
            var list = new List<FloorGetAll>();

            foreach(var item in data)
            {
                var checkAccount = _context.accounts.Where(x => x.id == item.account_id && !x.Deleted).FirstOrDefault();
                var checkWareHouser = _context.warehouses.Where(x => x.id == item.warehouse).FirstOrDefault();
                var checkArea = _context.areas.Where(x => x.floor == item.id && !x.Deleted).Count();
                var mapData = _mapper.Map<FloorGetAll>(item);
                mapData.Id = item.id;
                mapData.account_name = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.username;
                mapData.account_image = checkAccount == null ? Status.ACCOUNTNOTFOULD : checkAccount.image;
                mapData.warehouse_name = checkWareHouser == null ? Status.WAREHOUSERFOLDER : checkWareHouser.name;
                mapData.warehouse_image = checkWareHouser == null ? Status.WAREHOUSERFOLDER : checkWareHouser.image;
                mapData.locationEmty = item.quantityarea - checkArea;

                list.Add(mapData);
            }

            return list;
        }
        public async Task<PayLoad<object>> FindListByWarehouse(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.floors.Where(x => x.warehouse == id && !x.Deleted).ToList();


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

        public async Task<PayLoad<FloorGetAll>> FindOneId(int id)
        {
            try
            {
                
                var checkData = _context.floors.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                var checkWarehouse = _context.warehouses.Where(x => x.id == checkData.warehouse && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == checkData.account_id && !x.Deleted).FirstOrDefault();

                var mapData = _mapper.Map<FloorGetAll>(checkData);
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;
                mapData.warehouse_image = checkWarehouse.image;
                mapData.warehouse_name = checkWarehouse.name;
                mapData.Id = checkData.id;

                return await Task.FromResult(PayLoad<FloorGetAll>.Successfully(mapData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<FloorGetAll>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<FloorDTO>> Update(int id, FloorDTO floorDTO)
        {
            try
            {
                if (!checkFullQuantity(floorDTO.warehouse))
                    return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(Status.FULLQUANTITY));

                var user = _userService.name();
                var checkId = _context.floors.Where(x => x.id == id).FirstOrDefault();
                var checkName = _context.floors.Where(x => x.name == floorDTO.name && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();

                if (checkName != null || checkId == null)
                    return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(Status.DATANULL));

                if (!checkAreaFullQuantity(checkId.id, floorDTO.quantityarea))
                    return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkWarehouse = _context.warehouses.Where(x => x.id == floorDTO.warehouse && !x.Deleted).FirstOrDefault();
                if(floorDTO.image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.FLOOR + checkId.id.ToString(), _cloud);
                    uploadCloud.CloudInaryIFromAccount(floorDTO.image, TokenViewModel.FLOOR + checkId.id.ToString(), _cloud);
                    checkId.image = uploadCloud.Link;
                    checkId.publicid = uploadCloud.publicId;
                }

                checkId.name = floorDTO.name;
                checkId.quantityarea = floorDTO.quantityarea;
                checkId.warehouse = checkWarehouse.id;
                checkId.warehouse_id = checkWarehouse;
                checkId.UpdatedAt = DateTimeOffset.UtcNow;
                checkId.CretorEdit = checkAccount.username + " đã thay đổi lúc " + DateTimeOffset.UtcNow;

                _context.floors.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<FloorDTO>.Successfully(floorDTO));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<FloorDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkFullQuantity(int? id)
        {
            var checkWarehouse = _context.warehouses.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
            var checkQuantity = _context.floors.Where(x => x.warehouse == checkWarehouse.id && !x.Deleted).Count();

            if (checkQuantity > checkWarehouse.Numberoffloors)
                return false;

            return true;
        }

        private bool checkAreaFullQuantity(int? id, int? number)
        {
            var checkFloorArea = _context.areas.Where(x => x.floor == id && !x.Deleted).Count();
            if(number < checkFloorArea)
                return false;
            return true;
        }
    }
}
