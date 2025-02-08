using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.Runtime.InteropServices;

namespace quanlykhodl.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public SupplierService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<SupplierDTO>> Add(SupplierDTO data)
        {
            try
            {
                var user = _userService.name();
                var checkName = _context.suppliers.Where(x => x.name == data.name && !x.deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<SupplierDTO>.CreatedFail(Status.DATANULL));

                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();

                var mapData = _mapper.Map<Supplier>(data);
                mapData.account_id = checkAccount.id;
                mapData.accounts = checkAccount;

                _context.suppliers.Add(mapData);
                _context.SaveChanges();

                if(data.image != null)
                {
                    var dataNew = _context.suppliers.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                    uploadCloud.CloudInaryIFromAccount(data.image, TokenViewModel.SUPPLIER + dataNew.id.ToString(), _cloud);
                    dataNew.image = uploadCloud.Link;
                    dataNew.publicid = uploadCloud.publicId;

                    _context.suppliers.Update(dataNew);
                    _context.SaveChanges();
                }

                return await Task.FromResult(PayLoad<SupplierDTO>.Successfully(data));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<SupplierDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.suppliers.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                checkId.deleted = true;

                _context.suppliers.Update(checkId);
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
                var data = _context.suppliers.Where(x => !x.deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

                var pageList = new PageList<object>(LoadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<SupplierGetAll> LoadData(List<Supplier> data)
        {
            var list = new List<SupplierGetAll>();

            foreach (var item in data)
            {
                var mapData = _mapper.Map<SupplierGetAll>(item);
                mapData.Id = item.id;

                list.Add(mapData);  
            }
            return list;
        }

        public async Task<PayLoad<SupplierGetAll>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.suppliers.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<SupplierGetAll>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<SupplierGetAll>(checkId);

                return await Task.FromResult(PayLoad<SupplierGetAll>.Successfully(mapData));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<SupplierGetAll>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<SupplierDTO>> Update(int id, SupplierDTO data)
        {
            try
            {
                var checkId = _context.suppliers.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<SupplierDTO>.CreatedFail(Status.DATANULL));

                var mapdataUpdate = MapperData.GanData(checkId, data);
                if(data.image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.SUPPLIER + checkId.id.ToString(), _cloud);
                    uploadCloud.CloudInaryIFromAccount(data.image, TokenViewModel.SUPPLIER + checkId.id.ToString(), _cloud);
                    mapdataUpdate.image = uploadCloud.Link;
                    mapdataUpdate.publicid = uploadCloud.publicId;
                }

                _context.suppliers.Update(mapdataUpdate);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<SupplierDTO>.Successfully(data));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<SupplierDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
