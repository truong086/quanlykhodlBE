namespace quanlykhodl.ViewModel
{
    public class ProductDTO
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public List<IFormFile>? images { get; set; }
        public double price { get; set; }
        public string? DonViTinh { get; set; }
        public int quantity { get; set; }
        public int star { get; set; }
        public int? category_map { get; set; }
        public int? suppliers { get; set; }
        public int? shelfId { get; set; }
        public int? location { get; set; }
        public int? quantityshelf { get; set; }
    }

    public class ProductAddAreas
    {
        public int id_product { get; set; }
        public int id_shefl { get; set; }
        public int location { get; set; }
        public int quantity { get; set; }

    }
}
