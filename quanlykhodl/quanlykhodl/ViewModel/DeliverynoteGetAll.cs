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
        public int? TotalProduct { get; set; }
        public long? TotalQuantity { get; set; }
        public bool? isPercentage { get; set; }
        public bool? isAction { get; set; }
        public bool? isPack { get; set; }
        public List<productImportformAndDeliveerrynote>? products { get; set; }
    }

    public class WarehouseSalesPercentage
    {
        public int idWarerouse { get; set;}
        public string? warehouseName { get; set;}
        public string? warehouseImage { get; set;}

        public int idFloor { get; set; }
        public string? floorName { get; set; }
        public string? floorImage { get; set; }

        public int idArea { get; set; }
        public string? areaName { get; set; }
        public string? areaImage { get; set; }

        public int idShelf { get; set; }
        public string? shelfName { get; set; }
        public string? shelfImage { get; set; }
        public string? code { get; set; }
        public int? location { get; set; }
        public double Percentage { get; set; }
    }
}
