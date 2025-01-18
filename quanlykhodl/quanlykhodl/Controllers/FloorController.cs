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
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;
        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _floorService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<FloorGetAll>> FindOneId(int id)
        {
            return await _floorService.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(FindByWareHouser))]
        public async Task<PayLoad<object>> FindByWareHouser(int id, int page = 1, int pageSize = 20)
        {
            return await _floorService.FindListByWarehouse(id, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<FloorDTO>> Add([FromForm]FloorDTO data)
        {
            return await _floorService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<FloorDTO>> Update(int id, [FromForm] FloorDTO data)
        {
            return await _floorService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(DeleteData))]
        public async Task<PayLoad<string>> DeleteData(int id)
        {
            return await _floorService.Delete(id);
        }
    }
}
