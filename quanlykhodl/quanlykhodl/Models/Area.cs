using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Area : BaseEntity
	{
		public string? name { get; set; }
		public int? quantity { get; set; }
		public string? Status { get; set; }
		public string? code { get; set; }
        public string? image { get; set; }
        public string? publicid { get; set; }

        public int? account { get; set; }
		public int? floor { get; set; }
		public Account? account_id { get; set; }
		public Floor? floor_id { get; set; }
		public ICollection<product>? products { get; set; }
	}
}
