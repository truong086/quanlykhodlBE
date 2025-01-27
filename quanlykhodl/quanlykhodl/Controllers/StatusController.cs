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
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _statusService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindByAccount))]
        public async Task<PayLoad<object>> FindByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _statusService.FindByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindByPlan))]
        public async Task<PayLoad<object>> FindByPlan(int id)
        {
            return await _statusService.FindByPlan(id);
        }

        [HttpPut]
        [Route(nameof(UpdateStatusItem))]
        public async Task<PayLoad<StatusItemDTO>> UpdateStatusItem([FromForm]StatusItemDTO statusItemDTO)
        {
            return await _statusService.UpdateStatusItem(statusItemDTO);
        }

        [HttpPut]
        [Route(nameof(UpdateStatus))]
        public async Task<PayLoad<StatusWarehours>> UpdateStatus(StatusWarehours statusItemDTO)
        {
            return await _statusService.UpdateStatus(statusItemDTO);
        }
    }
}
