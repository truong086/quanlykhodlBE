using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IFloorService
    {
        Task<PayLoad<FloorDTO>> Add(FloorDTO floorDTO);
        Task<PayLoad<FloorDTO>> Update(int id, FloorDTO floorDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll (string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<FloorGetAll>> FindOneId (int id);
        Task<PayLoad<object>> FindListByWarehouse(int id, int page = 1, int pageSize = 20);
    }
}
