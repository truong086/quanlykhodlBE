using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Supplier : BaseEntity
	{
		public string? name { get; set; }
		public string? address { get; set; }
		public string? email { get; set; }
		public string? image { get; set; }
		public string? publicid { get; set; }
		public string? phone { get; set; }
		public int? account_id { get; set; }
		public Account? accounts { get; set; }

		public ICollection<product>? products { get; set; }
		public ICollection<productImportform>? productimportforms { get; set; }
	}
}
