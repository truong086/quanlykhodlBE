using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IRoleService
    {
        Task<PayLoad<RoleDTO>> Add(RoleDTO roleDTO);
        Task<PayLoad<RoleDTO>> Update(int id, RoleDTO roleDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<role>> FindOne(int id);

    }
}
