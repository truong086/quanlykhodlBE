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
        [Route(nameof(FindOneCodeProduct))]
        public async Task<PayLoad<object>> FindOneCodeProduct(string code)
        {
            return await _deliverynoteService.FindOneCodeProduct(code);
        }

        [HttpGet]
        [Route(nameof(FindOneCode))]
        public async Task<PayLoad<object>> FindOneCode(string code)
        {
            return await _deliverynoteService.FindOneCode(code);
        }

        [HttpGet]
        [Route(nameof(findOneById))]
        public async Task<PayLoad<object>> findOneById(int id)
        {
            return await _deliverynoteService.findOneById(id);
        }

        [HttpGet]
        [Route(nameof(FindAccountDelivenote))]
        public async Task<PayLoad<object>> FindAccountDelivenote(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindAccountDelivenote(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindNoPack))]
        public async Task<PayLoad<object>> FindNoPack(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindNoPack(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindOkPack))]
        public async Task<PayLoad<object>> FindOkPack(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindOkPack(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindOkPackNoIsAction))]
        public async Task<PayLoad<object>> FindOkPackNoIsAction(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindOkPackNoIsAction(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindOkPackOkIsAction))]
        public async Task<PayLoad<object>> FindOkPackOkIsAction(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindOkPackOkIsAction(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindNoPackOkIsAction))]
        public async Task<PayLoad<object>> FindNoPackOkIsAction(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindNoPackOkIsAction(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindNoPackNoIsAction))]
        public async Task<PayLoad<object>> FindNoPackNoIsAction(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindNoPackNoIsAction(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindNoAction))]
        public async Task<PayLoad<object>> FindNoAction(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindNoAction(name, page, pageSie);
        }

        [HttpGet]
        [Route(nameof(FindOkAction))]
        public async Task<PayLoad<object>> FindOkAction(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindOkAction(name, page, pageSie);
        }


        [HttpGet]
        [Route(nameof(FindAccountNoPack))]
        public async Task<PayLoad<object>> FindAccountNoPack(string? name, int page = 1, int pageSie = 20)
        {
            return await _deliverynoteService.FindAccountNoPack(name, page, pageSie);
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

        [HttpPut]
        [Route(nameof(UpdateActionLocation))]
        public async Task<PayLoad<uploadDataLocationArea>> UpdateActionLocation(uploadDataLocationArea data)
        {
            return await _deliverynoteService.UpdateActionLocation(data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _deliverynoteService.Delete(id);
        }
    }
}
