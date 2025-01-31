using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quanlykhodl.Common;
using quanlykhodl.Service;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalController : ControllerBase
    {
        private readonly IStatisticalService _statisticalService;
        public StatisticalController(IStatisticalService statisticalService)
        {
            _statisticalService = statisticalService;
        }

        [HttpGet]
        [Route(nameof(GetMonthlyProductStatistics))]
        public async Task<PayLoad<object>> GetMonthlyProductStatistics()
        {
            return await _statisticalService.GetMonthlyProductStatistics();
        }

        [HttpGet]
        [Route(nameof(GetTotalProductsSoldByMonth))]
        public async Task<PayLoad<object>> GetTotalProductsSoldByMonth()
        {
            return await _statisticalService.GetTotalProductsSoldByMonth();
        }

        [HttpGet]
        [Route(nameof(GetTotalProductsSoldByAccount))]
        public async Task<PayLoad<object>> GetTotalProductsSoldByAccount()
        {
            return await _statisticalService.GetTotalProductsSoldByAccount();
        }

        [HttpGet]
        [Route(nameof(GetTotalProductsSold))]
        public async Task<PayLoad<object>> GetTotalProductsSold()
        {
            return await _statisticalService.GetTotalProductsSold();
        }
        [HttpGet]
        [Route(nameof(GetTotalProductsSoldByCustomer))]
        public async Task<PayLoad<object>> GetTotalProductsSoldByCustomer()
        {
            return await _statisticalService.GetTotalProductsSoldByCustomer();
        }

        [HttpGet]
        [Route(nameof(GetDaylyProductStatistics))]
        public async Task<PayLoad<object>> GetDaylyProductStatistics()
        {
            return await _statisticalService.GetDaylyProductStatistics();
        }

        [HttpGet]
        [Route(nameof(GetHourselyProductStatistics))]
        public async Task<PayLoad<object>> GetHourselyProductStatistics()
        {
            return await _statisticalService.GetHourselyProductStatistics();
        }

        [HttpGet]
        [Route(nameof(SetTotalProductsSold))]
        public async Task<PayLoad<object>> SetTotalProductsSold()
        {
            return await _statisticalService.SetTotalProductsSold();
        }

        [HttpGet]
        [Route(nameof(SetTotalProductsSoldBySupplier))]
        public async Task<PayLoad<object>> SetTotalProductsSoldBySupplier()
        {
            return await _statisticalService.SetTotalProductsSoldBySupplier();
        }

        [HttpGet]
        [Route(nameof(SetTotalImportFromProductsSoldBySupplier))]
        public async Task<PayLoad<object>> SetTotalImportFromProductsSoldBySupplier()
        {
            return await _statisticalService.SetTotalImportFromProductsSoldBySupplier();
        }

        [HttpGet]
        [Route(nameof(SetDayAndMonthAnhYearlyProductStatistics))]
        public async Task<PayLoad<object>> SetDayAndMonthAnhYearlyProductStatistics()
        {
            return await _statisticalService.SetDayAndMonthAnhYearlyProductStatistics();
        }

        [HttpGet]
        [Route(nameof(SetTotalProductsSoldToDay))]
        public async Task<PayLoad<object>> SetTotalProductsSoldToDay()
        {
            return await _statisticalService.SetTotalProductsSoldToDay();
        }

    }
}
