namespace quanlykhodl.ViewModel
{
    public class StatusItemDTO
    {
        public int id_status { get; set; }
        public string? title { get; set; }
        public string? icon { get; set; }
        public List<IFormFile>? image { get; set; }
    }

    public class StatusWarehours
    {
        public int id_statuswarehourse { get; set; }
        public string? title { get; set; }
    }
}
