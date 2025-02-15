namespace quanlykhodl.ViewModel
{
    public class productShelf
    {
        public int Id { get; set; }
        public int quantity {  get; set; }
        public int totalLocation {  get; set; }
        public int totalLocationEmpty {  get; set; }
        public int totalLocatiEmpty {  get; set; }
        public int totalLocatiExsis {  get; set; }
        public string? name {  get; set; }
        public List<productLocationArea>? productLocationAreas { get; set; }
        public List<productLocationArea>? productPlans { get; set; }
        public Dictionary<int, int>? locationTotal {  get; set; }
        public List<WarehoursPlan>? warehoursPlans {  get; set; }
        public List<productInPlan>? productInPlans {  get; set; }

    }
    public class productLocationArea
    {
        public int Id { get; set; }
        public int Id_product { get; set; }
        public int Id_plan { get; set; }
        public string? titlePlan { get; set; }
        public string? name { get; set; }
        public string? code { get; set; }
        public string? image { get; set; }
        public string? category { get; set; }
        public string? category_image { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? supplier { get; set; }
        public string? supplier_image { get; set; }
        public int? location { get; set; }
        public int quantity { get; set; }
        public int Inventory { get; set; }
        public double price { get; set; }
        public IEnumerable<object>? images { get; set; }
    }

    public class WarehoursPlan
    {
        public string? warehours { get; set; }
        public string? floor { get; set; }
        public string? area { get; set; }
        public string? shelf { get; set; }
    }
}
