namespace quanlykhodl.ViewModel
{
    public class DeliverynoteGetAll
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public bool isRetailcustomers { get; set; }
        public string? nameAccountCreat { get; set; }
        public string? ImageAccountCreat { get; set; }
        public string? nameAccountBy { get; set; }
        public string? AddressAccountBy { get; set; }
        public string? EmailAccountBy { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public double total { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? code { get; set; }
        public int? Tax { get; set; }
        public bool? isPercentage { get; set; }
        public List<productImportformAndDeliveerrynote>? products { get; set; }
    }
}
