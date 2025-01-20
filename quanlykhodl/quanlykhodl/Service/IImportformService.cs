using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IImportformService
    {
        Task<PayLoad<ImportformDTO>> Add(ImportformDTO data);
        Task<PayLoad<ImportformUpdate>> Update(int id, ImportformUpdate data);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneId(int id);
        Task<PayLoad<object>> FindCode(string code);
    }
}
