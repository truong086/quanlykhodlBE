using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IStatisticalService
    {
        Task<PayLoad<object>> GetMonthlyProductStatistics();
        Task<PayLoad<object>> GetDaylyProductStatistics();
        Task<PayLoad<object>> GetHourselyProductStatistics();
        Task<PayLoad<object>> GetTotalProductsSoldByMonth();
        Task<PayLoad<object>> GetTotalProductsSoldByAccount();
        Task<PayLoad<object>> GetTotalProductsSoldByCustomer();
        Task<PayLoad<object>> GetTotalProductsSold();
        Task<PayLoad<object>> SetTotalProductsSoldBySupplier();
        Task<PayLoad<object>> SetTotalImportFromProductsSoldBySupplier();
        Task<PayLoad<object>> SetTotalProductsSold();
        Task<PayLoad<object>> SetTotalProductsSoldToDay();
        Task<PayLoad<object>> SetDayAndMonthAnhYearlyProductStatistics();
    }
}
