using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _categoryService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<CategoryGetAll>> FindOneId(int id)
        {
            return await _categoryService.FindOneId(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<CategoryDTO>> Add([FromForm]CategoryDTO data)
        {
            return await _categoryService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<CategoryDTO>> Update(int id, [FromForm]CategoryDTO data)
        {
            return await _categoryService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _categoryService.Delete(id);
        }
    }
}
