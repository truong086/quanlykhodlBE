using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface ISupplierService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<SupplierDTO>> Add(SupplierDTO data);
        Task<PayLoad<SupplierDTO>> Update(int id, SupplierDTO data);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<SupplierGetAll>> FindOneId(int id);

    }
}
