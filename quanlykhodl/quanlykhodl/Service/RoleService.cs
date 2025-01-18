using AutoMapper;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class RoleService : IRoleService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        public RoleService(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PayLoad<RoleDTO>> Add(RoleDTO roleDTO)
        {
            try
            {
                var checkName = _context.roles.Where(x => x.name.ToLower() == roleDTO.name.ToLower() && !x.Deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(Status.DATATONTAI));

                var dataMap = _mapper.Map<role>(roleDTO);
                dataMap.Deleted = false;

                _context.roles.Add(dataMap);

                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<RoleDTO>.Successfully(roleDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.roles.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.Deleted = true;
                checkId.UpdatedAt = DateTimeOffset.UtcNow;

                _context.roles.Update(checkId);

                await _context.SaveChangesAsync();

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
                var data = _context.roles.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) && !x.Deleted).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

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

        public async Task<PayLoad<role>> FindOne(int id)
        {
            try
            {
                var checkRole = _context.roles.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if(checkRole == null)
                    return await Task.FromResult(PayLoad<role>.CreatedFail(null));
                return await Task.FromResult(PayLoad<role>.Successfully(checkRole));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<role>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<RoleDTO>> Update(int id, RoleDTO roleDTO)
        {
            try
            {
                var checkId = _context.roles.Where(x => x.id == id).FirstOrDefault();
                var checkName = _context.roles.Where(x => x.name == roleDTO.name && x.name !=  roleDTO.name).FirstOrDefault();

                if (checkId == null)
                    return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(Status.DATANULL));

                if (checkName != null)
                    return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(Status.DATANULL));

                var mapDataUpdate = MapperData.GanData(checkId, roleDTO);
                mapDataUpdate.UpdatedAt = DateTimeOffset.UtcNow;

                _context.roles.Update(mapDataUpdate);

                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<RoleDTO>.Successfully(roleDTO));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail($"{ex.Message}"));
            }
        }
    }
}
