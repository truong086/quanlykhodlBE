using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IAreaService
    {
        Task<PayLoad<AreaDTO>> Add (AreaDTO areaDTO);
        Task<PayLoad<AreaDTO>> Update (int id, AreaDTO areaDTO);
        Task<PayLoad<string>> Delete (int id);
        Task<PayLoad<object>> FindAll (string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneId (int id);
        Task<PayLoad<object>> FindByFloor (int id);
        
    }
}
