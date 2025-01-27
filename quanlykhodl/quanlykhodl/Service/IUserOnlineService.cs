using quanlykhodl.Common;

namespace quanlykhodl.Service
{
    public interface IUserOnlineService
    {
        Task<PayLoad<object>> FindAll();
    }
}
