using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<RoleDTO>> Add(RoleDTO roleDTO)
        {
            return await _roleService.Add(roleDTO);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<RoleDTO>> Update(int id, RoleDTO roleDTO)
        {
            return await _roleService.Update(id, roleDTO);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _roleService.Delete(id);
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _roleService.FindAll(name, page, pageSize);
        }
    }
}
