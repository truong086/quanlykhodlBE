namespace quanlykhodl.ViewModel
{
    public class AreaGetAll
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public int? quantity { get; set; }
        public int? quantityEmtity { get; set; }
        public int? totalLocationExsis { get; set; }
        public string? Status { get; set; }
        public string? code { get; set; }
        public string? image { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? floor_image { get; set; }
        public string? floor_name { get; set; }
        public productArea? productArea { get; set; }
        public List<totalQuantityUsed>? totalQuantityUseds { get; set; }
    }

    public class totalQuantityUsed
    {
        public int location { get; set; }
        public int quantity { get; set; }
        public int quantityUsed { get; set; }
    }
}
