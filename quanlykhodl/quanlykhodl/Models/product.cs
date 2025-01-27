using quanlykhodl.Common;
using System.Drawing;
using System.Numerics;

namespace quanlykhodl.Models
{
	public class product : BaseEntity
	{
		public string? title { get; set; }
		public string? description { get; set; }
		public double price { get; set; }
		public string? DonViTinh { get; set; }
		public int quantity { get; set; }
		public int star { get; set; }
		//public int location { get; set; }
		public string? code { get; set; }

		public int? category_map { get; set; }
		public int? account_map { get; set; }
		public int? suppliers { get; set; }
		public category? categoryid123 { get; set; }
		public Supplier? supplier_id { get; set; }
		public Account? account { get; set; }

		public virtual ICollection<ImageProduct>? imageProducts { get; set; }
		public virtual ICollection<productDeliverynote>? productDeliverynotes { get; set; }
		public virtual ICollection<productImportform>? productImportforms { get; set; }
		public virtual ICollection<productlocation>? Productlocations { get; set; }
		//public virtual ICollection<PrepareToExport>? PrepareToExports { get; set; }

	}
}
