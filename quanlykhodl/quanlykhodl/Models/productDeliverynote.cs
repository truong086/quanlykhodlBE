using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class productDeliverynote : BaseEntity
	{
		public int? deliverynote { get; set; }
		public int? product_map { get; set; }
		public int quantity {  get; set; }
		public Deliverynote? deliverynote_id1 { get; set; }
		public product? product { get; set; }
	}
}
