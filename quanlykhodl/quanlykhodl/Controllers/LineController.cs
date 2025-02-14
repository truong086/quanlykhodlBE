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
    public class LineController : ControllerBase
    {
        private readonly ILineService _lineService;
        public LineController(ILineService lineService)
        {
            _lineService = lineService;            
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<LineDTO>> Add(LineDTO lineDTO)
        {
            return await _lineService.Add(lineDTO);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<LineDTO>> Update(int id, LineDTO lineDTO)
        {
            return await _lineService.Update(id, lineDTO);
        }


        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _lineService.Delete(id);
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _lineService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _lineService.FindOne(id);
        }
    }
}
