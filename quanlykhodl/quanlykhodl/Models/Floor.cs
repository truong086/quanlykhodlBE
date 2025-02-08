using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Floor : BaseEntity
	{
		public string? name { get; set; }
		public int? quantityarea { get; set; }
		public string? code { get; set; }
        public string? image { get; set; }
        public string? publicid { get; set; }
        public int? warehouse { get; set; }
        public int? account_id { get; set; }
        public Warehouse? warehouse_id { get; set; }
        public accounts? accounts { get; set; }
        public ICollection<areas>? areas { get; set; }
	}
}
