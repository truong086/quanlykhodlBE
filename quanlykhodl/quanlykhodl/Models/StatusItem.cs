using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class StatusItem : BaseEntity
	{
		public string? title { get; set; }
		public string? icon { get; set; }
		public int? warehousetransferstatus { get; set; }
		public Warehousetransferstatus? Warehousetransferstatus_id { get; set; }
		public ICollection<imagestatusitem>? imagestatusitems { get; set; }
	}
}
