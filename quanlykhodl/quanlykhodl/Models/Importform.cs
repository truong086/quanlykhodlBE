using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Importform : BaseEntity
	{
		public string? tite { get; set; }
		public string? description { get; set; }
		public int? account_idmap { get; set; }
		public accounts? account_id { get; set; }
		public bool isproductnew { get; set; }
		public bool isaction { get; set; }
		public int? quantity { get; set; }
		public int? tax { get; set; }
		public int? actualquantity { get; set; }
		public bool? ispercentage{ get; set; }
		public double price { get; set; }
		public double total { get; set; }
		public string? deliveryaddress { get; set; }
		public string? code { get; set; }
		public ICollection<productImportform>? productImportforms { get; set; }
	}
}