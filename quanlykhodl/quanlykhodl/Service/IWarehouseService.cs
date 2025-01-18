using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IWarehouseService
    {
        Task<PayLoad<WarehouseDTO>> Add(WarehouseDTO data);
        Task<PayLoad<WarehouseDTO>> Update(int id, WarehouseDTO data);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<WarehouseGetAll>> FindOneId(int id);
    }
}
