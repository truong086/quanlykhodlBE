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
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _areaService.FinAll(name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<AreaDTO>> Add([FromForm]AreaDTO data)
        {
            return await _areaService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<AreaDTO>> Update(int id, [FromForm] AreaDTO data)
        {
            return await _areaService.Update(id, data);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<AreaGetAll>> FindOneId(int id)
        {
            return await _areaService.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(FindByFloor))]
        public async Task<PayLoad<object>> FindByFloor(int id, int page = 1, int pageSize = 20)
        {
            return await _areaService.FindOneFloor(id, page, pageSize);
        }
    }
}
