using quanlykhodl.Common;

namespace quanlykhodl.Service
{
    public interface IMessageService
    {
        Task<PayLoad<object>> FindAll(int userId1);
    }
}
