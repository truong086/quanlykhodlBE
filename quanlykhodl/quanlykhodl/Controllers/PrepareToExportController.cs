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
    public class PrepareToExportController : ControllerBase
    {
        private readonly IPrepareToExportService _prepareToExportService;
        public PrepareToExportController(IPrepareToExportService prepareToExportService)
        {
            _prepareToExportService = prepareToExportService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _prepareToExportService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneCode))]
        public async Task<PayLoad<object>> FindOneCode(string code)
        {
            return await _prepareToExportService.FindOneCode(code);
        }

        [HttpGet]
        [Route(nameof(FindAccount))]
        public async Task<PayLoad<object>> FindAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _prepareToExportService.FindAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _prepareToExportService.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(FindDataNoIsCheck))]
        public async Task<PayLoad<object>> FindDataNoIsCheck(string? name, int page = 1, int pageSize = 20)
        {
            return await _prepareToExportService.FindDataNoIsCheck(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindCode))]
        public async Task<PayLoad<object>> FindCode(string code, string? name, int page = 1, int pageSize = 20)
        {
            return await _prepareToExportService.FindCode(code, name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<PrepareToExportDTO>> Add(PrepareToExportDTO data)
        {
            return await _prepareToExportService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<PrepareToExportDTO>> Update(int id, PrepareToExportDTO data)
        {
            return await _prepareToExportService.Udpate(id, data);
        }

        [HttpPut]
        [Route(nameof(UdpateCheck))]
        public async Task<PayLoad<string>> UdpateCheck(int id)
        {
            return await _prepareToExportService.UdpateCheck(id);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _prepareToExportService.Deletet(id);
        }
    }
}
