using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IPrepareToExportService
    {
        Task<PayLoad<PrepareToExportDTO>> Add(PrepareToExportDTO data);
        Task<PayLoad<PrepareToExportDTO>> Udpate(int id, PrepareToExportDTO data);
        Task<PayLoad<string>> Deletet(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneId(int id);
        Task<PayLoad<object>> FindOneCode(string id);
        Task<PayLoad<string>> UdpateCheck(int id);
        Task<PayLoad<object>> FindDataNoIsCheck(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindCode(string code, string? name, int page = 1, int pageSize = 20);
        
    }
}
