using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Floor : BaseEntity
	{
		public string? name { get; set; }
		public int? quantityarea { get; set; }
		public string? code { get; set; }
		public int? warehouse { get; set; }
		public Warehouse? warehouse_id { get; set; }

		public ICollection<Area>? areas { get; set; }
		public ICollection<product>? products { get; set; }
	}
}
