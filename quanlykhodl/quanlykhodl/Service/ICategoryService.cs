using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface ICategoryService
    {
        Task<PayLoad<CategoryDTO>> Add(CategoryDTO data);
        Task<PayLoad<CategoryDTO>> Update(int id, CategoryDTO data);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<CategoryGetAll>> FindOneId(int id);
    }
}
