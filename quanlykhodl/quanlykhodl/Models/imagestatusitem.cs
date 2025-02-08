using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class imagestatusitem : BaseEntity
	{
		public int? statusitemmap { get; set; }
		public StatusItem? statusItem_id { get; set; }
		public string? image { get; set; }
		public string? publicid { get; set; }
	}
}
