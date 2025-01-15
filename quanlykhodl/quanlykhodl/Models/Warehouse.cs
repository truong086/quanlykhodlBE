using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Warehouse : BaseEntity
	{
		public string? name { get; set; }
		public int? Numberoffloors { get; set; }
		public string? Street { get; set; }
		public string? District { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
		public string? address { get; set; }
		public string? code { get; set; }

		public int? account_map { get; set; }
		public Account? account { get; set; }

		public ICollection<Floor>? floors { get; set; }
	}
}
