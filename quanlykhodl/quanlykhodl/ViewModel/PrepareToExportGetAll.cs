namespace quanlykhodl.ViewModel
{
    public class PrepareToExportGetAll
    {
        public int id {  get; set; }
        public int? id_product {  get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public int quantity { get; set; }
        //public int location { get; set; }
        public string? code { get; set; }
        public List<areaFloorWarehourseDelivenote>? areaFloorWarehourseDelivenotes { get; set; }
    }

    public class areaFloorWarehourseDelivenote
    {
        public string? area { get; set; }
        public string? floor { get; set; }
        public string? warehourse { get; set; }
        public int? location { get; set; }
        public string? codeLocation { get; set; }
    }
}
