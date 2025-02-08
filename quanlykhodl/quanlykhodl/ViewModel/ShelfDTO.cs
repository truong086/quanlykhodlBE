namespace quanlykhodl.ViewModel
{
    public class ShelfDTO
    {
        public string? name { get; set; }
        public int? quantity { get; set; }
        public string? Status { get; set; }
        public string? image { get; set; }
        public int? area { get; set; }
        public int? max { get; set; }
        public List<locationExceptionsDTO>? locationExceptionsDTOs { get; set; }
    }

    public class locationExceptionsDTO
    {
        public int? location { get; set; }
        public int? quantity { get; set; }
    }
}
