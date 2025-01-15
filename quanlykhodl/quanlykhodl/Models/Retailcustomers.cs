using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Retailcustomers : BaseEntity
	{
		public int? deliverynote { get; set; }
		public Deliverynote? deliverynote_id1 { get; set; }
		public string? name { get; set; }
		public string? address { get; set; }
		public string? phone { get; set; }
		public string? email { get; set; }
	}
}
