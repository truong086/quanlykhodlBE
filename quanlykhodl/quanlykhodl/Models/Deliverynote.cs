using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Deliverynote : BaseEntity
	{
		public string? title { get; set; }
		public string? description { get; set; }
		public int? accountmap { get; set; }
		public Account? account { get; set; }
		public bool isRetailcustomers { get; set; }
		public double price { get; set; }
		public int quantity { get; set; }
		public double total { get; set; }
		public string? DeliveryAddress { get; set; }
		public string? code { get; set; }
		public ICollection<productDeliverynote>? productDeliverynotes { get; set; }
		public ICollection<Retailcustomers>? retailcustomers { get; set; }
	}
}
