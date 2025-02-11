using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IUserTokenAppService
    {
        Task<PayLoad<UserTokenAppDTO>> AddToken(UserTokenAppDTO userTokenAppDTO);
        Task<PayLoad<UserTokenAppDTO>> RegisterTopic(UserTokenAppDTO userTokenAppDTO);
        Task<PayLoad<string>> SendNotify();
        Task<PayLoad<string>> DeleteData();
    }
}
