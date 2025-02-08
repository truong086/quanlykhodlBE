using quanlykhodl.Models;

namespace quanlykhodl.ViewModel
{
    public class ImportformDTO
    {
        public string? tite { get; set; }
        public string? description { get; set; }
        public bool isProductNew { get; set; }
        public List<productOld>? productOlds { get; set; }
        public List<productNew>? productNew { get; set; }
        public int? quantity { get; set; }
        public double price { get; set; }
        public double total { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? Tax { get; set; }
        public bool? isPercentage { get; set; }
    }

    public class productOld
    {
        public int id_product { get; set; }
        public int quantity { get; set; }
        public int? shelfId { get; set; }
        public int? location { get; set; }
        public int? supplier { get; set; }
    }
    public class productNew
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public int quantity { get; set; }
        public int quantityLocation { get; set; }
        public int? category_map { get; set; }
        public int? suppliers { get; set; }
        public int? shelfId { get; set; }
        public int? location { get; set; }
        public List<string>? image { get; set; }
    }

    
    public class ImportformUpdate
    {
        public bool? isPercentage {  set; get; }
        public int? Tax { get; set; }
        public double total { get; set; }

    }

    public class ImportformUpdateCode
    {
        public string? code { get; set; }
        public int? ActualQuantity { get; set; }

    }
}
