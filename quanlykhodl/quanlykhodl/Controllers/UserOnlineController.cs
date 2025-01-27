using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Service;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserOnlineController : ControllerBase
    {
        private readonly IUserOnlineService _userOnlineService;
        public UserOnlineController(IUserOnlineService userOnlineService)
        {
            _userOnlineService = userOnlineService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll()
        {
            return await _userOnlineService.FindAll();
        }
    }
}
