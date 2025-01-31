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
        Task<PayLoad<string>> CheckPack(updatePack data);
        Task<PayLoad<object>> FindOneCodeProduct(string code);

        Task<PayLoad<object>> FindNoPack(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOkPack(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOkPackNoIsAction(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOkPackOkIsAction(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindNoPackOkIsAction(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindNoPackNoIsAction(string? name, int page = 1, int pageSize = 20);

        Task<PayLoad<object>> FindNoAction(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOkAction(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAccountNoPack(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAccountDelivenote(string? name, int page = 1, int pageSize = 20);

    }
}
