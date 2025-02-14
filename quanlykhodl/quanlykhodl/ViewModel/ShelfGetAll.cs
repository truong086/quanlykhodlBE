namespace quanlykhodl.ViewModel
{
    public class ShelfGetAll
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public int? quantity { get; set; }
        public int? quantityEmtity { get; set; }
        public int? totalLocationExsis { get; set; }
        public int? max { get; set; }
        public string? Status { get; set; }
        public string? code { get; set; }
        public string? image { get; set; }
        public string? imageShelf { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public string? Area_image { get; set; }
        public string? Area_name { get; set; }
        public int? lineidsdata { get; set; }
        public string? linenames { get; set; }
        public int? Id_Area { get; set; }
        public productShelf? productShefl { get; set; }
        public List<quantityException>? quantityExceptions { get; set; }
        public List<totalQuantityUsed>? totalQuantityUseds { get; set; }
        public List<productInPlan>? productInPlans { get; set; }
    }

    public class productInPlan
    {
        public string? title { get; set; }
        public int? locationNew { get; set; }
        public int? locationOld { get; set; }
        public int? shelfsOld { get; set; }
        public int? shelfsNew { get; set; }
    }
    public class totalQuantityUsed
    {
        public int location { get; set; }
        public int quantity { get; set; }
        public int quantityUsed { get; set; }
    }

    public class quantityException
    {
        public int? location { get; set; }
        public int? quantity { get; set; }
    }
}
