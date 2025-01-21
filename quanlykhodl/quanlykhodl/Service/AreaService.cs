using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class AreaService : IAreaService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public AreaService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<AreaDTO>> Add(AreaDTO areaDTO)
        {
            try
            {
                var user = _userService.name();
                var checkName = _context.areas.Where(x => x.name == areaDTO.name && !x.Deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                var checkFloor = _context.floors.Where(x => x.id == areaDTO.floor && !x.Deleted).FirstOrDefault();
                if (checkFloor == null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if (!checkFullQuantity(checkFloor, checkFloor.id))
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();

                var mapData = _mapper.Map<Area>(areaDTO);
                mapData.code = RanDomCode.geneAction(8);
                mapData.account = checkAccount.id;
                mapData.account_id = checkAccount;
                mapData.floor = checkFloor.id;
                mapData.floor_id = checkFloor;

                _context.areas.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.areas.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (areaDTO.image != null)
                {
                    uploadCloud.CloudInaryIFromAccount(areaDTO.image, TokenViewModel.AREA + dataNew.id.ToString(), _cloud);
                    dataNew.image = uploadCloud.Link;
                    dataNew.publicid = uploadCloud.publicId;

                    _context.areas.Update(dataNew);
                    _context.SaveChanges();
                }

                if(areaDTO.locationExceptionsDTOs != null)
                {
                    updateLocationExcep(areaDTO.locationExceptionsDTOs, dataNew);
                }

                return await Task.FromResult(PayLoad<AreaDTO>.Successfully(areaDTO));


            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(ex.Message));
            }
        }

        private void updateLocationExcep(List<locationExceptionsDTO> data, Area area)
        {
            var list = new List<int>();
            foreach (var item in data)
            {
                if (!list.Contains(item.location))
                {
                    if (item.location < area.quantity)
                    {
                        var localExceps = new LocationException
                        {
                            area = area,
                            id_area = area.id,
                            location = item.location,
                            max = item.quantity
                        };

                        _context.locationExceptions.Add(localExceps);
                        _context.SaveChanges();

                        list.Add(item.location);
                    }
                }
            }
        }
        private bool checkFullQuantity(Floor floor, int id)
        {
            var checkAreaQuantity = _context.areas.Where(x => x.floor == id && !x.Deleted).Count();
            if (floor.quantityarea < checkAreaQuantity)
                return false;

            return true;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkData = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                checkData.Deleted = true;

                _context.areas.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FinAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.areas.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) && !x.Deleted).ToList();

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

        private List<AreaGetAll> LoadData(List<Area> data)
        {
            var list = new List<AreaGetAll>();

            foreach (var item in data)
            {
                var checkAccount = _context.accounts.Where(x => x.id == item.account && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == item.floor && !x.Deleted).FirstOrDefault();
                var checkProductLocation = _context.productlocations.Where(x => x.id_area == item.id && !x.Deleted).Count();
                var mapData = _mapper.Map<AreaGetAll>(item);
                mapData.Id = item.id;
                mapData.floor_name = checkFloor.name;
                mapData.floor_image = checkFloor.image;
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;
                mapData.quantityEmtity = item.quantity - checkProductLocation;

                list.Add(mapData);

            }
            return list;
        }

        public async Task<PayLoad<object>> FindOneFloor(int id, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.areas.Where(x => x.floor == id && !x.Deleted).ToList();

                var pageList = new PageList<object>(LoadData(data), page - 1, pageSize);

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

        public async Task<PayLoad<AreaGetAll>> FindOneId(int id)
        {
            try
            {
                var checkData = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<AreaGetAll>.CreatedFail(Status.DATANULL));
                var checkAccount = _context.accounts.Where(x => x.id == checkData.account && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == checkData.floor && !x.Deleted).FirstOrDefault();

                var mapData = _mapper.Map<AreaGetAll>(checkData);
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;
                mapData.floor_image = checkFloor.image;
                mapData.floor_name = checkFloor.name;

                return await Task.FromResult(PayLoad<AreaGetAll>.Successfully(mapData));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaGetAll>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<AreaDTO>> Update(int id, AreaDTO areaDTO)
        {
            try
            {
                var user = _userService.name();
                var checkId = _context.areas.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == areaDTO.floor && !x.Deleted).FirstOrDefault();
                var checkName = _context.areas.Where(x => x.name == areaDTO.name && !x.Deleted).FirstOrDefault();

                if (checkId == null || checkAccount == null || checkFloor == null || checkName != null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if (!checkFullQuantity(checkFloor, checkFloor.id))
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if (!checkQuantityproductInArea(checkId.id, areaDTO.quantity))
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                if(areaDTO.image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.AREA + checkId.id.ToString(), _cloud);
                    uploadCloud.CloudInaryIFromAccount(areaDTO.image, TokenViewModel.AREA + checkId.id.ToString(), _cloud);
                    checkId.image = uploadCloud.Link;
                    checkId.publicid = uploadCloud.publicId;
                }

                if(areaDTO.locationExceptionsDTOs != null)
                {
                    var checkLocationExcep = _context.locationExceptions.Where(x => x.id_area == checkId.id && !x.Deleted).ToList();
                    if(checkLocationExcep.Count > 0 && checkLocationExcep != null)
                    {
                        _context.locationExceptions.RemoveRange(checkLocationExcep);
                        _context.SaveChanges();
                    }

                    updateLocationExcep(areaDTO.locationExceptionsDTOs, checkId);

                }
                checkId.quantity = areaDTO.quantity;
                checkId.Status = areaDTO.Status;
                checkId.floor = checkFloor.id;
                checkId.floor_id = checkFloor;
                checkId.name = areaDTO.name;

                _context.areas.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<AreaDTO>.Successfully(areaDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkQuantityproductInArea(int id, int? quantity)
        {
            var checkProducArea = _context.productlocations.Where(x => x.id_area ==  id && !x.Deleted).ToList();
            if(checkProducArea.Any())
            {
                foreach (var item in checkProducArea)
                {
                    var checkLocation = _context.productlocations.Where(x => x.id_product == item.id && !x.Deleted).OrderByDescending(x => x.location).FirstOrDefault();
                    if (quantity < checkLocation.location)
                        return false;
                }
            }

            return true;
        }
    }
}
