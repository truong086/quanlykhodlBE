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
    public class DeliverynoteController : ControllerBase
    {
        private readonly IDeliverynoteService _deliverynoteService;
        public DeliverynoteController(IDeliverynoteService deliverynoteService)
        {
            _deliverynoteService = deliverynoteService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _deliverynoteService.findAll(name, page, pageSize);    
        }

        [HttpGet]
        [Route(nameof(findOneById))]
        public async Task<PayLoad<object>> findOneById(int id)
        {
            return await _deliverynoteService.findOneById(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<DeliverynoteDTO>> Add(DeliverynoteDTO data)
        {
            return await _deliverynoteService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<ImportformUpdate>> Update(int id, ImportformUpdate data)
        {
            return await _deliverynoteService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _deliverynoteService.Delete(id);
        }
    }
}
