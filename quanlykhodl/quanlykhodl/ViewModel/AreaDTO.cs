namespace quanlykhodl.ViewModel
{
    public class AreaDTO
    {
        public string? name { get; set; }
        public int? quantity { get; set; }
        public string? Status { get; set; }
        public IFormFile? image { get; set; }
        public int? floor { get; set; }
        public int? max { get; set; }
        public List<locationExceptionsDTO>? locationExceptionsDTOs { get; set; }
    }

    public class locationExceptionsDTO
    {
        public int location { get; set; }
        public int quantity { get; set; }
    }
}
