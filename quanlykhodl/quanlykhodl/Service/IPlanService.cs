using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IPlanService
    {
        Task<PayLoad<PlanDTO>> Add(PlanDTO planDTO);
        Task<PayLoad<PlanAllWarehoursDTO>> AddAllWarehours(PlanAllWarehoursDTO planDTO);
        Task<PayLoad<PlanDTO>> Update(int id, PlanDTO planDTO);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindConfirmationAndConsentByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindConfirmationAndConsentAdmin(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindConfirmationAndNoConsentByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindConfirmationAndNoConsentAdmin(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindNoConfirmationByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindNoConfirmationAdmin(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllByAccountPlanNoConfirmByDate();
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<PlanGetAll>> FindOne(int id);
        Task<PayLoad<bool>> UpdatePlanConfirmation(ConfirmationPlan data);
        Task<PayLoad<object>> FindDoneByAdmin(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindDoneByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindConfirmationByAccount(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllDataByDate(searchDatetimePlan datetimePlan, int page = 1, int pageSize = 20);
        byte[] FindAllDataByDateExcel(searchDatetimePlan datetimePlan);
        List<PlanGetAll> FindALlDataExcel(searchDatetimePlan datetimePlan);
    }
}
