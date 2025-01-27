using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IStatusService
    {
        Task<PayLoad<StatusItemDTO>> UpdateStatusItem(StatusItemDTO statusItemDTO);
        Task<PayLoad<StatusWarehours>> UpdateStatus(StatusWarehours statusItemDTO);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindByPlan(int id);
    }
}
