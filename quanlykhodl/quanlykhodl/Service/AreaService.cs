using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var checkName = _context.areas.Where(x => x.name == areaDTO.name && !x.deleted).FirstOrDefault();
                var checkFloor = _context.floors.Where(x => x.id == areaDTO.floor_map && !x.deleted).FirstOrDefault();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();

                if(checkAccount == null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATATONTAI));

                if (checkName != null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATATONTAI));

                if(checkFloor == null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                var dataMap = _mapper.Map<areas>(areaDTO);
                dataMap.floor = checkFloor.id;
                dataMap.floor_id = checkFloor;
                dataMap.account = checkAccount;
                dataMap.account_id = checkAccount.id;

                _context.areas.Add(dataMap);
                _context.SaveChanges();

                var dataNew = _context.areas.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                if(areaDTO.imagearea != null)
                {
                    uploadCloud.CloudInaryIFromAccount(areaDTO.imagearea, TokenViewModel.AREA + dataNew.id.ToString(), _cloud);
                    dataNew.image = uploadCloud.Link;
                    dataNew.publicId = uploadCloud.publicId;
                }

                dataNew.code = RanDomCode.geneAction(8) + dataNew.id.ToString();

                _context.areas.Update(dataNew);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<AreaDTO>.Successfully(areaDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.areas.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if(checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.areas.Where(x => !x.deleted).ToList();

                if(!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);
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

        private List<AreaGetAll> loadData (List<areas> data)
        {
            var list = new List<AreaGetAll>();

            foreach (var item in data)
            {
                list.Add(findOneDataMap(item));
            }

            return list;
        }

        private AreaGetAll findOneDataMap(areas item)
        {
            var checkFloor = _context.floors.Where(x => x.id == item.floor && !x.deleted).FirstOrDefault();
            var checkAccount = _context.accounts.Where(x => x.id == item.account_id && !x.deleted).FirstOrDefault();

            var dataItem = _mapper.Map<AreaGetAll>(item);
            dataItem.accountName = checkAccount == null ? Status.ACCOUNTFAILD : checkAccount.username;
            dataItem.accountImage = checkAccount == null ? Status.ACCOUNTFAILD : checkAccount.image;
            dataItem.floorName = checkFloor == null ? Status.ACCOUNTFAILD : checkFloor.name;
            dataItem.floorImage = checkFloor == null ? Status.ACCOUNTFAILD : checkFloor.image;
            dataItem.floorCode = checkFloor == null ? Status.ACCOUNTFAILD : checkFloor.code;
            dataItem.floorId = checkFloor == null ? 0 : checkFloor.id;

            return dataItem;
        }

        public async Task<PayLoad<object>> FindByFloor(int id)
        {
            try
            {
                var checkAreaOfFloor = _context.areas.Where(x => x.floor == id && !x.deleted).ToList();
                if(checkAreaOfFloor.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = loadData(checkAreaOfFloor)
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.areas.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(findOneDataMap(checkId)));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<AreaDTO>> Update(int id, AreaDTO areaDTO)
        {
            try
            {
                var checkId = _context.areas.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                var checkShelfQuantity = _context.shelfs.Where(x => x.line == checkId.id && !x.deleted).Count();
                if(checkShelfQuantity > areaDTO.storage)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkFloor = _context.floors.Where(x => x.id == areaDTO.floor_map && !x.deleted).FirstOrDefault();   
                if(checkFloor == null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATANULL));

                var checkName = _context.areas.Where(x => x.name == areaDTO.name && x.name != checkId.name).FirstOrDefault();
                if(checkName != null)
                    return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(Status.DATATONTAI));

                var dataMap = MapperData.GanData(checkId, areaDTO);
                dataMap.floor = checkFloor.id;
                dataMap.floor_id = checkFloor;
                if(areaDTO.imagearea != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.AREA + checkId.id.ToString(), _cloud);
                    uploadCloud.CloudInaryIFromAccount(areaDTO.imagearea, TokenViewModel.AREA + checkId.ToString(), _cloud);
                    dataMap.image = uploadCloud.Link;
                    dataMap.publicId = uploadCloud.publicId;
                }

                _context.areas.Update(dataMap);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<AreaDTO>.Successfully(areaDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AreaDTO>.CreatedFail(ex.Message));
            }
        }

    }
}
