using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IShelfService
    {
        Task<PayLoad<ShelfDTO>> Add(ShelfDTO areaDTO);
        Task<PayLoad<ShelfDTO>> Update(int id, ShelfDTO areaDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FinAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<ShelfGetAll>> FindOneId(int id);
        Task<PayLoad<object>> FindOneArea(int id, int page = 1, int pageSize = 20);
    }
}
