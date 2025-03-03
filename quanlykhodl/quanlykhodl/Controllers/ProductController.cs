﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _productService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindCodeLocation))]
        public async Task<PayLoad<object>> FindCodeLocation(string code)
        {
            return await _productService.FindCodeLocation(code);
        }

        [HttpGet]
        [Route(nameof(FindOneByCategory))]
        public async Task<PayLoad<object>> FindOneByCategory(int id, int page = 1, int pageSize = 20)
        {
            return await _productService.FindOneByCategory(id, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneByArea))]
        public async Task<PayLoad<object>> FindOneByArea(int id)
        {
            return await _productService.FindOneByShelf(id);
        }

        [HttpGet]
        [Route(nameof(FindOneCode))]
        public async Task<PayLoad<object>> FindOneCode(string id)
        {
            return await _productService.FindCode(id);
        }

        [HttpGet]
        [Route(nameof(FindOneBySipplier))]
        public async Task<PayLoad<object>> FindOneBySipplier(int id, int page = 1, int pageSize = 20)
        {
            return await _productService.FindOneBySipplier(id, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneProductInWarehourse))]
        public async Task<PayLoad<object>> FindOneProductInWarehourse(int id)
        {
            return await _productService.FindOneProductInWarehourse(id);
        }

        [HttpGet]
        [Route(nameof(FindAllProductInWarehourse))]
        public async Task<PayLoad<object>> FindAllProductInWarehourse(int id, int page = 1, int pageSize = 20)
        {
            return await _productService.FindAllProductInWarehourse(id, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneById))]
        public async Task<PayLoad<ProductGetAll>> FindOneById(int id)
        {
            return await _productService.FindOneById(id);
        }

        [HttpGet]
        [Route(nameof(FindAllProductInFloorAndArea))]
        public async Task<PayLoad<object>> FindAllProductInFloorAndArea(int id_floor, int id_area, int page = 1, int pageSize = 20)
        {
            return await _productService.FindAllProductInFloorAndArea(id_floor, id_area, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllProductInWarehouseAndFloorAndArea))]
        public async Task<PayLoad<object>> FindAllProductInWarehouseAndFloorAndArea(int id_warehouse, int id_floor, int id_area, int page = 1, int pageSize = 20)
        {
            return await _productService.FindAllProductInWarehouseAndFloorAndArea(id_warehouse, id_floor, id_area, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllProductInWarehouseAndFloorAndAreaAndShelf))]
        public async Task<PayLoad<object>> FindAllProductInWarehouseAndFloorAndAreaAndShelf(int id_warehouse, int id_floor, int id_area, int id_shelf, int page = 1, int pageSize = 20)
        {
            return await _productService.FindAllProductInWarehouseAndFloorAndAreaAndShelf(id_warehouse, id_floor, id_area, id_shelf, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllProductInFloor))]
        public async Task<PayLoad<object>> FindAllProductInFloor(int id_floor, int page = 1, int pageSize = 20)
        {
            return await _productService.FindAllProductInFloor(id_floor, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(FindAllProductSearch))]
        public async Task<PayLoad<object>> FindAllProductSearch(SerchData data)
        {
            return await _productService.FindAllProductSearch(data);
        }


        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<ProductDTO>> Add([FromForm]ProductDTO data)
        {
            return await _productService.Add(data);
        }

        [HttpPost]
        [Route(nameof(AddArea))]
        public async Task<PayLoad<ProductAddAreas>> AddArea(ProductAddAreas data)
        {
            return await _productService.AddArea(data);
        }

        [HttpPost]
        [Route(nameof(checkLocationTotal))]
        public async Task<PayLoad<bool>> checkLocationTotal(checkLocationExsis data)
        {
            return await _productService.checkLocationTotal(data);
        }

        [HttpPut]
        [Route(nameof(UpdateArea))]
        public async Task<PayLoad<ProductAddAreas>> UpdateArea(int id, ProductAddAreas productDTOs)
        {
            return await _productService.UpdateArea(id, productDTOs);
        }

        [HttpPut]
        [Route(nameof(UpdateAreaQuantity))]
        public async Task<PayLoad<ProductAddAreas>> UpdateAreaQuantity(int id, ProductAddAreas productDTOs)
        {
            return await _productService.UpdateAreaQuantity(id, productDTOs);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<ProductDTO>> Update(int id, [FromForm]ProductDTO productDTO)
        {
            return await _productService.Update(id, productDTO);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _productService.Delete(id);
        }

        [HttpPost]
        [Route(nameof(checkLocation))]
        public async Task<PayLoad<bool>> checkLocation(checkLocation data)
        {
            return await _productService.checkLocation(data);
        }

        [HttpPost]
        [Route(nameof(checkLocationProductExsis))]
        public async Task<PayLoad<object>> checkLocationProductExsis(checkLocation data)
        {
            return await _productService.checkLocationProductExsis(data);
        }

        [HttpPost]
        [Route(nameof(FindAllDataByDateExcel))]
        public IActionResult FindAllDataByDateExcel()
        {
            var dataByte = _productService.FindAllDataByDateExcel();

            return File(dataByte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductLocation.xlsx");
        }
    }
}
