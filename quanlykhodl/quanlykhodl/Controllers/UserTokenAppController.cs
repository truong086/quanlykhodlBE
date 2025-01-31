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
    public class UserTokenAppController : ControllerBase
    {
        private readonly IUserTokenAppService _userTokenAppService;
        public UserTokenAppController(IUserTokenAppService userTokenAppService)
        {
            _userTokenAppService = userTokenAppService;
        }

        [HttpPost]
        [Route(nameof(AddToken))]
        public async Task<PayLoad<UserTokenAppDTO>> AddToken(UserTokenAppDTO data)
        {
            return await _userTokenAppService.AddToken(data);
        }

        [HttpPost]
        [Route(nameof(RegisterTopic))]
        public async Task<PayLoad<UserTokenAppDTO>> RegisterTopic(UserTokenAppDTO data)
        {
            return await _userTokenAppService.RegisterTopic(data);
        }

        [HttpPost]
        [Route(nameof(SendNotify))]
        public async Task<PayLoad<string>> SendNotify()
        {
            return await _userTokenAppService.SendNotify();
        }
    }
}
