using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface ILineService
    {
        Task<PayLoad<LineDTO>> Add(LineDTO lineDTO);
        Task<PayLoad<LineDTO>> Update(int id, LineDTO lineDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOne(int id);

    }
}
