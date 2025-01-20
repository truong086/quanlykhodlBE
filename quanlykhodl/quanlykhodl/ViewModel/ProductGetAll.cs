namespace quanlykhodl.ViewModel
{
    public class ProductGetAll
    {
        public int Id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public List<string>? images { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public string? categoryName { get; set; }
        public string? categoryImage { get; set; }
        public int quantity { get; set; }
        public int star { get; set; }
        public List<listAreaOfproduct>? listAreaOfproducts { get; set; }
    }

    public class listAreaOfproduct
    {
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? addressWarehouse { get; set; }
        public string? warehouse_name { get; set; }
        public string? warehouse_image { get; set; }
        public string? floor_name { get; set; }
        public string? floor_image { get; set; }
        public string? area_name { get; set; }
        public string? area_image { get; set; }
        public int? quantity { get; set; }
        public int? Id_productlocation { get; set; }
        public int? location { get; set; }
    }

    public class ProductOneLocation
    {
        public int Id { get; set; }
        public int Id_product { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public List<string>? images { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public int quantity { get; set; }
        public int star { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? addressWarehouse { get; set; }
        public string? warehouse_name { get; set; }
        public string? warehouse_image { get; set; }
        public string? floor_name { get; set; }
        public string? floor_image { get; set; }
        public string? area_name { get; set; }
        public string? area_image { get; set; }
        public int? quantityArea { get; set; }
    }

    public class checkLocation
    {
        public int id_Area { get; set; }
        public int id_product { get; set; }
        public int location { get; set; }
    }
}
