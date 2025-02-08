namespace quanlykhodl.ViewModel
{
    public class ProductSalesByMonth
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<string>? image { get; set; }
        public List<producSales>? producSalesData { get; set; }
    }

    public class producSales
    {
        public int? id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public List<string>? images { get; set; }
    }

    public class ProductSalesByDay
    {
        public int? Day { get; set; }
        public int? hourse { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<string>? image { get; set; }
        public IEnumerable<object>? data { get; set; }
        public DateTimeOffset? dateNow { get; set; }
        public List<producSales>? producSalesData { get; set; }
    }

    public class MonthlySales
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalProductsSold { get; set; }
    }

    public class MonthlySalesAccountByProduct
    {
        public string? username { get; set; }
        public string? image { get; set; }
        public string? email { get; set; }
        public int? total { get; set; }
    }

    public class LySalesAccountByProductCustomer
    {
        public int? id { get; set; }
        public string? username { get; set; }
        public string? image { get; set; }
        public string? email { get; set; }
        public string? customer_name { get; set; }
        public string? customer_address { get; set; }
        public string? customer_email { get; set; }
        public int? total { get; set; }
        public List<producSales>? producSalesData { get; set; }
    }

    public class TotalProduct
    {
        public int? id { get; set; }
        public string? title { get; set; }
        public List<string>? image { get; set; }
        public string? code { get; set; }
        public int? total { get; set; }
    }

    public class TotalProductImportFrom
    {
        public int? Day { get; set; }
        public int? Hourse { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Total { get; set; }
        public string? title { get; set; }
        public string? code { get; set; }
        public IEnumerable<object>? data { get; set; }
    }

    public class TotalProductSupplier
    {
        public int? Day { get; set; }
        public int? Hourse { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Total { get; set; }
        public int? id { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public string? image { get; set; }
        public IEnumerable<object>? data { get; set; }
    }

    public class TotalProductQuantity
    {
        public int? Day { get; set; }
        public int? Hourse { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Total { get; set; }
        public int? Id { get; set; }
        public string? title { get; set; }
        public string? code { get; set; }
        public string? description { get; set; }
        public string? account_name { get; set; }
        public string? account_image { get; set; }
        public IEnumerable<string>? image { get; set; }
    }
}
