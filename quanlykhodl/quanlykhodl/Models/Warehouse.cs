using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Warehouse : BaseEntity
	{
		public string? name { get; set; }
		public int? numberoffloors { get; set; }
		public string? street { get; set; }
		public string? district { get; set; }
		public string? city { get; set; }
		public string? country { get; set; }
		public string? address { get; set; }
		public string? code { get; set; }
		public string? image { get; set; }
		public string? publicid { get; set; }

		public int? account_map { get; set; }
		public accounts? account { get; set; }

		public ICollection<Floor>? floors { get; set; }
	}
}
