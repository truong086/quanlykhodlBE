using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<AccountDTO>> Add([FromForm]AccountDTO accountDTO)
        {
            return await _accountService.Add(accountDTO);
        }

        [AllowAnonymous]
        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<AccountUpdate>> Update(int id, [FromForm] AccountUpdate accountDTO)
        {
            return await _accountService.Update(id, accountDTO);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(LoadOTP))]
        public async Task<PayLoad<string>> LoadOTP(reLoadOtp data)
        {
            return await _accountService.ReloadOTP(data);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(ForgotPassword))]
        public async Task<PayLoad<string>> ForgotPassword(forgotPassword data)
        {
            return await _accountService.ForgotPassword(data);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Action))]
        public async Task<PayLoad<string>> Action(ActionAccount data)
        {
            return await _accountService.Action(data);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _accountService.FindAll(name, page, pageSize);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindAllAccountOnline))]
        public async Task<PayLoad<object>> FindAllAccountOnline()
        {
            return await _accountService.FindAllAccountOnline();
        }

        [HttpGet]
        [Route(nameof(Showrofile))]
        public async Task<PayLoad<object>> Showrofile()
        {
            return await _accountService.Showrofile();
        }

        [AllowAnonymous]
        [HttpPut]
        [Route(nameof(UpdateRole))]
        public async Task<PayLoad<AccountUpdateRole>> UpdateRole(AccountUpdateRole data)
        {
            return await _accountService.UpdateRole(data);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(checkCode))]
        public async Task<PayLoad<string>> checkCode(ActionAccount data)
        {
            return await _accountService.CheckCode(data);
        }

        [AllowAnonymous]
        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _accountService.Delete(id);
        }

        [HttpGet]
        [Route(nameof(FindAllToken))]
        public async Task<PayLoad<object>> FindAllToken()
        {
            return await _accountService.FindAllToken();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(LoginData))]
        public async Task<PayLoad<ReturnLogin>> LoginData(Login data)
        {
            return await _accountService.LoginPage(data);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(UpdatePassworData))]
        public async Task<PayLoad<string>> UpdatePassworData(updatatePasswordAccount data)
        {
            return await _accountService.updatePasswords(data);
        }
    }
}
