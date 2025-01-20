using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public CategoryService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<CategoryDTO>> Add(CategoryDTO data)
        {
            try
            {
                var user = _userService.name();
                var checkName = _context.categories.Where(x => x.name == data.name && !x.Deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(Status.DATANULL));

                var checkAccount = _context.accounts.Where(x => x.id == Convert.ToInt32(user) && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<category>(data);
                mapData.account = checkAccount;
                mapData.account_Id = checkAccount.id;

                _context.categories.Add(mapData);
                _context.SaveChanges();

                if(data.image != null)
                {
                    var dataNew = _context.categories.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    uploadCloud.CloudInaryIFromAccount(data.image, TokenViewModel.CATEGORY + dataNew.id.ToString(), _cloud);
                    dataNew.image = uploadCloud.Link;
                    dataNew.public_id = uploadCloud.publicId;

                    _context.categories.Update(dataNew);
                    _context.SaveChanges();
                }

                return await Task.FromResult(PayLoad<CategoryDTO>.Successfully(data));

            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.categories.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                checkId.Deleted = true;

                _context.categories.Update(checkId);
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
                var data = _context.categories.Where(x => !x.Deleted).ToList();

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

        private List<CategoryGetAll> LoadData(List<category> data)
        {
            var list = new List<CategoryGetAll>();

            foreach (var item in data)
            {
                var checkAccount = _context.accounts.Where(x => x.id ==  item.account_Id && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<CategoryGetAll>(item);
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;

                list.Add(mapData);
            }
            return list;
        }
        public async Task<PayLoad<CategoryGetAll>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.categories.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<CategoryGetAll>.CreatedFail(Status.DATANULL));

                var checkAccount = _context.accounts.Where(x => x.id == checkId.account_Id && !x.Deleted).FirstOrDefault();
                var mapData = _mapper.Map<CategoryGetAll>(checkId);
                mapData.account_name = checkAccount.username;
                mapData.account_image = checkAccount.image;

                return await Task.FromResult(PayLoad<CategoryGetAll>.Successfully(mapData));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<CategoryGetAll>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<CategoryDTO>> Update(int id, CategoryDTO data)
        {
            try
            {
                var checkId = _context.categories.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                var checkName = _context.categories.Where(x => x.name == data.name && !x.Deleted).FirstOrDefault();
                if (checkId == null || checkName != null)
                    return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(Status.DATANULL));

                var mapDataUpdate = MapperData.GanData(checkId, data);
                if (data.image != null)
                {
                    uploadCloud.DeleteAllImageAndFolder(TokenViewModel.CATEGORY + checkId.id.ToString(), _cloud);
                    uploadCloud.CloudInaryIFromAccount(data.image, TokenViewModel.CATEGORY + checkId.id.ToString(), _cloud);
                    mapDataUpdate.image = uploadCloud.Link;
                    mapDataUpdate.public_id = uploadCloud.publicId;
                }

                _context.categories.Update(mapDataUpdate);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<CategoryDTO>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
