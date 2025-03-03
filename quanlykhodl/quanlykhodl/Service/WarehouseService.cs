﻿using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public WarehouseService(DBContext context, IMapper mapper, IOptions<Cloud> cloud, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _cloud = cloud.Value;
            _userService = userService;

        }
        public async Task<PayLoad<WarehouseDTO>> Add(WarehouseDTO data)
        {
            try
            {
                var user = _userService.name();
                var idConver = Convert.ToInt32(user);
                var checkName = _context.warehouses.Where(x => x.name == data.name && !x.deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.DATANULL));

                var checkCodeExsis = _context.warehouses.Where(x => x.code == data.code && !x.deleted).FirstOrDefault();
                if(checkCodeExsis != null)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.DATATONTAI));

                var checkAccount = _context.accounts.Where(x => x.id == idConver && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Warehouse>(data);
                //mapData.code = RanDomCode.geneAction(8);
                mapData.account = checkAccount;
                mapData.account_map = checkAccount.id;

                _context.warehouses.Add(mapData);
                await _context.SaveChangesAsync();

                if (data.Image != null)
                {
                    var dataNew = _context.warehouses.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                    uploadCloud.CloudInaryIFromAccount(data.Image, Status.WAREHOUSERFOLDER + dataNew.id.ToString(), _cloud);
                    dataNew.image = uploadCloud.Link;
                    dataNew.publicid = uploadCloud.publicId;

                    _context.warehouses.Update(dataNew);
                    await _context.SaveChangesAsync();
                }
                
                return await Task.FromResult(PayLoad<WarehouseDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.warehouses.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;

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
                var data = _context.warehouses.Where(x => !x.deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

                var pageList = new PageList<object>(LoadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<WarehouseGetAll> LoadData(List<Warehouse> data)
        {
            var list = new List<WarehouseGetAll>();

            foreach(var item in data)
            {
                var checkAccountId = checkAccount(item.account_map);
                var checkFloorTotal = _context.floors.Where(x => x.warehouse == item.id && !x.deleted).Count();
                var mapData = _mapper.Map<WarehouseGetAll>(item);
                mapData.account_name = checkAccountId == null ? Status.ACCOUNTNOTFOULD : checkAccountId.username;
                mapData.Id = item.id;
                mapData.account_image = checkAccountId == null ? Status.ACCOUNTNOTFOULD : checkAccountId.image;
                mapData.NumberoffloorsEmty = item.numberoffloors - checkFloorTotal;
                list.Add(mapData);
            }

            return list;
        }

        private accounts checkAccount(int? id)
        {
            var checkId = _context.accounts.Where(x => x.id == id && !x.deleted).FirstOrDefault();

            return checkId == null ? null : checkId;
        }
        public async Task<PayLoad<WarehouseGetAll>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.warehouses.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<WarehouseGetAll>.CreatedFail(Status.DATANULL));

                var checkAccountId = checkAccount(checkId.account_map);

                var mapData = _mapper.Map<WarehouseGetAll>(checkId);
                mapData.account_name = checkAccountId.username;
                mapData.account_image = checkAccountId.image;
                mapData.address = checkId.address;

                return await Task.FromResult(PayLoad<WarehouseGetAll>.Successfully(mapData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<WarehouseGetAll>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<WarehouseDTO>> Update(int id, WarehouseDTO data)
        {
            try
            {
                var user = _userService.name();
                var checkId = _context.warehouses.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.DATANULL));

                var checkCodeExsis = _context.warehouses.Where(x => x.code == data.code && x.id != checkId.id && !x.deleted).FirstOrDefault();
                if(checkCodeExsis != null)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.DATATONTAI));

                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.DATANULL));

                var checkQuantityFloor = _context.floors.Where(x => x.warehouse == checkId.id).Count();
                if (data.Numberoffloors < checkQuantityFloor)
                    return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(Status.FULLQUANTITY));

                var mapDataUpdate = MapperData.GanData(checkId, data);
                if(data.Image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(Status.WAREHOUSERFOLDER + checkId.id, _cloud);
                    uploadCloud.CloudInaryIFromAccount(data.Image, Status.WAREHOUSERFOLDER + checkId.id, _cloud);

                    mapDataUpdate.image = uploadCloud.Link;
                    mapDataUpdate.publicid = uploadCloud.publicId;
                }
                mapDataUpdate.updatedat = DateTimeOffset.UtcNow;
                mapDataUpdate.cretoredit = checkAccount.username + " đã sủa bản ghi lúc " + DateTimeOffset.UtcNow;

                _context.warehouses.Update(mapDataUpdate);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<WarehouseDTO>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<WarehouseDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
