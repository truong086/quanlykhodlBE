using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using quanlykhodl.Common;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;
using quanlykhodl.Models;

namespace quanlykhodl.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly DBContext _context;
        public PlanController(IPlanService planService, DBContext context)
        {
            _planService = planService;
            _context = context;
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

        [HttpPost]
        [Route(nameof(ExportToExcel))]
        public IActionResult ExportToExcel(searchDatetimePlan datetimePlan)
        {
            var dataList = _planService.FindALlDataExcel(datetimePlan);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Title";
                worksheet.Cells[1, 3].Value = "status";
                worksheet.Cells[1, 4].Value = "localtionNew";
                worksheet.Cells[1, 5].Value = "isConfirmation";
                worksheet.Cells[1, 6].Value = "receiver_name";
                worksheet.Cells[1, 7].Value = "localtionOld";
                worksheet.Cells[1, 8].Value = "localtionOldCode";
                worksheet.Cells[1, 9].Value = "localtionNewCode";
                worksheet.Cells[1, 10].Value = "warehouseOld";
                worksheet.Cells[1, 11].Value = "areaOld";
                worksheet.Cells[1, 12].Value = "shelfOld";
                worksheet.Cells[1, 13].Value = "floorOld";
                worksheet.Cells[1, 14].Value = "warehouse";
                worksheet.Cells[1, 15].Value = "area";
                worksheet.Cells[1, 16].Value = "shelf";
                worksheet.Cells[1, 17].Value = "floor";
                worksheet.Cells[1, 18].Value = "account_creatPlan";
                worksheet.Cells[1, 19].Value = "updatedAt";
                worksheet.Cells[1, 20].Value = "codeWarehourseNew";

                // Định dạng tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 20])
                {
                    range.Style.Font.Bold = true; // Chữ in đậm
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid; // Nền đặc
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Nền xám nhạt
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa nội dung
                }

                // Đổ dữ liệu vào file Excel
                int row = 2;
                foreach (var product in dataList)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.title;
                    worksheet.Cells[row, 3].Value = product.status;
                    worksheet.Cells[row, 4].Value = product.localtionNew;
                    worksheet.Cells[row, 5].Value = product.isConfirmation;
                    worksheet.Cells[row, 6].Value = product.Receiver_name;
                    worksheet.Cells[row, 7].Value = product.localtionOld;
                    worksheet.Cells[row, 8].Value = product.localtionOldCode;
                    worksheet.Cells[row, 9].Value = product.localtionNewCode;
                    worksheet.Cells[row, 10].Value = product.warehouseOld;
                    worksheet.Cells[row, 11].Value = product.areaOld;
                    worksheet.Cells[row, 12].Value = product.shelfOld;
                    worksheet.Cells[row, 13].Value = product.floorOld;
                    worksheet.Cells[row, 14].Value = product.warehouse;
                    worksheet.Cells[row, 15].Value = product.area;
                    worksheet.Cells[row, 16].Value = product.shelf;
                    worksheet.Cells[row, 17].Value = product.floor;
                    worksheet.Cells[row, 18].Value = product.Account_creatPlan;
                    worksheet.Cells[row, 19].Value = product.UpdatedAt;
                    worksheet.Cells[row, 20].Value = product.CodeWarehourseNew;
                    row++;
                }

                worksheet.Cells.AutoFitColumns(); // Tự động chỉnh độ rộng cột

                // Trả về file Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DataExcel.xlsx");
            }
        }
    }
}
