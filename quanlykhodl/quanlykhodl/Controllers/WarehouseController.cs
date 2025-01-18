using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _warehouseService.FindAll(name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<WarehouseDTO>> Add([FromForm]WarehouseDTO data)
        {
            return await _warehouseService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<WarehouseDTO>> Update(int id, [FromForm]WarehouseDTO data)
        {
            return await _warehouseService.Update(id, data);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<WarehouseGetAll>> FindOneId(int id)
        {
            return await _warehouseService.FindOneId(id);
        }

        [HttpDelete]
        [Route(nameof(DeleteData))]
        public async Task<PayLoad<string>> DeleteData(int id)
        {
            return await _warehouseService.Delete(id);
        }
    }
}
