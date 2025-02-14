using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Shelf : BaseEntity
	{
		public string? name { get; set; }
		public int? quantity { get; set; }
		public int? max { get; set; }
		public string? status { get; set; }
		public string? code { get; set; }
        public string? image { get; set; }
        public string? publicid { get; set; }
        public int? account { get; set; }
		public int? line { get; set; }
		public accounts? account_id { get; set; }
		public Line? line_id { get; set; }
		public virtual ICollection<productlocation>? Productlocations { get; set; }
		public virtual ICollection<LocationException>? LocationExceptions { get; set; }
		public virtual ICollection<Codelocation>? Codelocations { get; set; }
		public virtual ICollection<productImportform>? ProductImportforms { get; set; }
	}
}
