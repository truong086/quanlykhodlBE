using quanlykhodl.Models;

namespace quanlykhodl.ViewModel
{
    public class ImportformGetAll
    {
        public int Id { get; set; }
        public string? tite { get; set; }
        public string? description { get; set; }
        public string? accountName { get; set; }
        public string? accountImage { get; set; }
        public bool isProductNew { get; set; }
        public int? quantity { get; set; }
        public double price { get; set; }
        public double total { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? code { get; set; }
        public int? Tax { get; set; }
        public bool? isPercentage { get; set; }
        public List<productImportformAndDeliveerrynote>? products { get; set; }
    }

    public class productImportformAndDeliveerrynote
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public int quantity { get; set; }
        public int star { get; set; }
        public string? code { get; set; }
        public string? category_map { get; set; }
        public string? category_image { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? suppliers { get; set; }
        public string? suppliersImage { get; set; }
        public List<listArea>? data { get; set; }
        
    }

    public class listArea
    {
        public string? area { get; set; }
        public string? floor { get; set; }
        public string? warehourse { get; set; }
        public int? location { get; set; }
    }
}
