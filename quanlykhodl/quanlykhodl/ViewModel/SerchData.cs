namespace quanlykhodl.ViewModel
{
    public class SerchData
    {
        public int? idWarehouse { get; set; }
        public int? idFloor { get; set; }
        public int? idArea { get; set; }
        public int? idShelf { get; set; }
        public int? supplier { get; set; }
        public int? category { get; set; }
        public int? pricefrom { get; set; }
        public int? priceto { get; set; }
        public string? name { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
    }
}
