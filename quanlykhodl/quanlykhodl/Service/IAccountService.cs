using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IAccountService
    {
        Task<PayLoad<AccountDTO>> Add(AccountDTO accountDTO);
        Task<PayLoad<ReturnLogin>> LoginPage(Login accountDTO);
        Task<PayLoad<AccountUpdate>> Update(int id, AccountUpdate accountDTO);
        Task<PayLoad<string>> CheckToken(string token);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> Showrofile();
        Task<PayLoad<AccountUpdateRole>> UpdateRole(AccountUpdateRole data);
        Task<PayLoad<object>> FindOne(int id);
        Task<PayLoad<object>> FindAllAccountOnline();
        Task<PayLoad<string>> DeleteToken(Token token, string? status);
        Task<PayLoad<string>> DeleteAccountNoAction();
        Task<PayLoad<string>> ReloadOTP(reLoadOtp data);
        Task<PayLoad<string>> ForgotPassword(forgotPassword data);
        Task<PayLoad<string>> Action(ActionAccount data);
        Task<PayLoad<string>> CheckCode(ActionAccount data);
        Task<PayLoad<object>> FindAllToken();
        Task<PayLoad<string>> updatePasswords(updatatePasswordAccount data);
        Task<PayLoad<string>> LogOut();
    }
}
