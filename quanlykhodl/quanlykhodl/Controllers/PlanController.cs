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
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }


        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<PlanDTO>> Add(PlanDTO planDTO)
        {
            return await _planService.Add(planDTO);
        }
        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<PlanDTO>> Update(int id, PlanDTO planDTO)
        {
            return await _planService.Update(id, planDTO);
        }

        [HttpPut]
        [Route(nameof(UpdatePlanConfirmation))]
        public async Task<PayLoad<bool>> UpdatePlanConfirmation(ConfirmationPlan data)
        {
            return await _planService.UpdatePlanConfirmation(data);
        }

        [HttpDelete]
        [Route(nameof(DeleteData))]
        public async Task<PayLoad<string>> DeleteData(int id)
        {
            return await _planService.Delete(id);
        }
        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindAll(name, page, pageSize);    
        }

        [HttpGet]
        [Route(nameof(FindConfirmationAndConsentByAccount))]
        public async Task<PayLoad<object>> FindConfirmationAndConsentByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindConfirmationAndConsentByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindConfirmationAndConsentAdmin))]
        public async Task<PayLoad<object>> FindConfirmationAndConsentAdmin(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindConfirmationAndConsentAdmin(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindConfirmationAndNoConsentByAccount))]
        public async Task<PayLoad<object>> FindConfirmationAndNoConsentByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindConfirmationAndNoConsentByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindConfirmationAndNoConsentAdmin))]
        public async Task<PayLoad<object>> FindConfirmationAndNoConsentAdmin(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindConfirmationAndNoConsentAdmin(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindNoConfirmationByAccount))]
        public async Task<PayLoad<object>> FindNoConfirmationByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindNoConfirmationByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindNoConfirmationAdmin))]
        public async Task<PayLoad<object>> FindNoConfirmationAdmin(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindNoConfirmationAdmin(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOne))]
        public async Task<PayLoad<PlanGetAll>> FindOne(int id)
        {
            return await _planService.FindOne(id);
        }

        [HttpGet]
        [Route(nameof(SearchDate))]
        public async Task<PayLoad<object>> SearchDate()
        {
            return await _planService.FindAllByAccountPlanNoConfirmByDate();
        }

        [HttpGet]
        [Route(nameof(FindDoneByAdmin))]
        public async Task<PayLoad<object>> FindDoneByAdmin(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindDoneByAdmin(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindDoneByAccount))]
        public async Task<PayLoad<object>> FindDoneByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindDoneByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindConfirmationByAccount))]
        public async Task<PayLoad<object>> FindConfirmationByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _planService.FindConfirmationByAccount(name, page, pageSize);
        }
    }
}
