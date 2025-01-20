using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IAreaService
    {
        Task<PayLoad<AreaDTO>> Add(AreaDTO areaDTO);
        Task<PayLoad<AreaDTO>> Update(int id, AreaDTO areaDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FinAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<AreaGetAll>> FindOneId(int id);
        Task<PayLoad<object>> FindOneFloor(int id, int page = 1, int pageSize = 20);
    }
}
