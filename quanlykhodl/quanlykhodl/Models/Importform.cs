using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Importform : BaseEntity
	{
		public string? tite { get; set; }
		public string? description { get; set; }
		public int? account_idMap { get; set; }
		public Account? account_id { get; set; }
		public bool isProductNew { get; set; }
		public bool isAction { get; set; }
		public int? quantity { get; set; }
		public int? Tax { get; set; }
		public int? ActualQuantity { get; set; }
		public bool? isPercentage{ get; set; }
		public double price { get; set; }
		public double total { get; set; }
		public string? DeliveryAddress { get; set; }
		public string? code { get; set; }
		public ICollection<productImportform>? productImportforms { get; set; }
	}
}
