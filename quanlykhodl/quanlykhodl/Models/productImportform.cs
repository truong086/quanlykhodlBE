using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class productImportform : BaseEntity
	{
		public int? importform { get; set; }
		public int? product { get; set; }
		public int quantity { get; set; }
        public int? supplier { get; set; }
		public int? location { get; set; }
		public int? shelf_id { get; set; }
		public int? productlocation_id { get; set; }
		public bool isaction { get; set; }
		public string? code { get; set; }
		public productlocation? productlocations { get; set; }
		public Shelf? shelfs { get; set; }
        public Supplier? supplier_id { get; set; }
        public Importform? importform_id1 { get; set; }
		public product? products { get; set; }
	}
}
