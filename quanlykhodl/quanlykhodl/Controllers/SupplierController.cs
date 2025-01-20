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
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name , int page = 1, int pageSize = 20)
        {
            return await _supplierService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<SupplierGetAll>> FindOneId(int id)
        {
            return await _supplierService.FindOneId(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<SupplierDTO>> Add([FromForm]SupplierDTO data)
        {
            return await _supplierService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<SupplierDTO>> Update(int id, [FromForm] SupplierDTO data)
        {
            return await _supplierService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _supplierService.Delete(id);
        }
    }
}
