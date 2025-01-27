using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IDeliverynoteService
    {
        Task<PayLoad<object>> findAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> findOneById(int id);
        Task<PayLoad<object>> FindOneCode(string code);
        Task<PayLoad<DeliverynoteDTO>> Add(DeliverynoteDTO data);
        Task<PayLoad<ImportformUpdate>> Update(int id, ImportformUpdate data);
        Task<PayLoad<uploadDataLocationArea>> UpdateActionLocation(uploadDataLocationArea data);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindOneCodeProduct(string code);
        Task<PayLoad<object>> FindAccountDelivenote(string? name, int page = 1, int pageSize = 20);

    }
}
