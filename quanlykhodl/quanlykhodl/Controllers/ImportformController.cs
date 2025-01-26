using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ImportformController : ControllerBase
    {
        private readonly IImportformService _importformService;
        public ImportformController(IImportformService importformService)
        {
            _importformService = importformService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _importformService.FindAll(name, page, pageSize);
        }
        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _importformService.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(FindOneCode))]
        public async Task<PayLoad<object>> FindOneCode(string code)
        {
            return await _importformService.FindCode(code);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<ImportformDTO>> Add(ImportformDTO data)
        {
            return await _importformService.Add(data);
        }

        [HttpPost]
        [Route(nameof(FindCodeProductImportFrom))]
        public async Task<PayLoad<object>> FindCodeProductImportFrom(string data)
        {
            return await _importformService.FindCodeProductImportFrom(data);
        }

        [HttpPost]
        [Route(nameof(UpdateCode))]
        public async Task<PayLoad<string>> UpdateCode(ImportformUpdateCode data)
        {
            return await _importformService.UpdateCode(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<ImportformUpdate>> Update(int id, ImportformUpdate data)
        {
            return await _importformService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _importformService.Delete(id);
        }
    }
}
