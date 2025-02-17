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
        public string? code { get; set; }
        public int? categoryId { get; set; }
        public string? categoryName { get; set; }
        public string? categoryImage { get; set; }
        public int? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public int quantity { get; set; }
        public int star { get; set; }
        public List<listAreaOfproduct>? listAreaOfproducts { get; set; }
        public List<historyProductLocation>? historyProductLocations { get; set; }
        public List<OneDataShelfOfProduct>? oneDataShelfOfProducts { get; set; }
    }

    public class historyProductLocation
    {
        public string? type { get; set; }
        public int? idWarehouse { get; set; }
        public string? Warehouse_name { get; set; }
        public string? Warehouse_image { get; set; }

        public int? idFloor { get; set; }
        public string? Floor_name { get; set; }
        public string? Floor_image { get; set; }

        public int? idArea { get; set; }
        public string? Area_name { get; set; }
        public string? Area_image { get; set; }

        public int? idShelf { get; set; }
        public string? Shelf_name { get; set; }
        public string? Shelf_image { get; set; }
        public string? code { get; set; }
        public int? lcoation { get; set; }

        public DateTimeOffset? createAt { get; set; }
    }
    public class OneDataShelfOfProduct
    {
        public int Id { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? addressWarehouse { get; set; }
        public string? warehouse_name { get; set; }
        public string? warehouse_image { get; set; }
        public int quantity { get; set; }
        public List<listAreaOfproduct>? listShelfOfproducts { get; set; }
    }

    public class productWarehouse
    {
        public int Id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public List<string>? images { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public string? categoryName { get; set; }
        public string? categoryImage { get; set; }
        public string? code { get; set; }
        public int quantity { get; set; }
        public int star { get; set; }
        public string? warehouse_name { get; set; }
        public string? warehouse_image { get; set; }
        public string? floor_name { get; set; }
        public string? floor_image { get; set; }
        public string? area_name { get; set; }
        public string? area_image { get; set; }
        public string? shelf_name { get; set; }
        public string? shelf_image { get; set; }
        public string? line_name { get; set; }
        public string? suppliers_name { get; set; }
        public string? suppliers_image { get; set; }
        public string? codeLocation { get; set; }
        public int? quantityLocaton { get; set; }
        public int? location { get; set; }
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
        public string? shelf_name { get; set; }
        public string? shelf_image { get; set; }
        public string? line_name { get; set; }
        public string? code { get; set; }
        public int? quantity { get; set; }
        public int? idShelf { get; set; }
        public int? idArea { get; set; }
        public int? idFloor { get; set; }
        public int? idWarehouse { get; set; }
        public int? Id_productlocation { get; set; }
        public int? location { get; set; }
        public int? MaxlocationExceps { get; set; }
        public int? MaxlocationShelf { get; set; }
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
        public string? code { get; set; }
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
        public string? shelf_name { get; set; }
        public string? shelf_image { get; set; }
        public string? codeLocation { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public string? categoryImage { get; set; }
        public string? categoryName { get; set; }
        public int? quantityArea { get; set; }
        public int? TotalLocationEmty { get; set; }
    }

    public class checkLocation
    {
        public int id_Shelf { get; set; }
        public int id_product { get; set; }
        public int location { get; set; }
    }

    public class checkLocationExsis
    {
        public int id_shelf { get; set; }
        public int location { get; set; }
    }
}
