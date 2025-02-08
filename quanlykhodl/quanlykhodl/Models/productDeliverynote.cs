using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class productDeliverynote : BaseEntity
	{
		public int? deliverynote { get; set; }
		public int? product_map { get; set; }
		public int quantity {  get; set; }
		public int? location {  get; set; }
		public string? code {  get; set; }
        public int? shelf_id { get; set; }
        public int? productlocation_id { get; set; }
        public productlocation? productlocations { get; set; }
        public Shelf? shelfs { get; set; }
        public Deliverynote? deliverynote_id1 { get; set; }
		public product? product { get; set; }
	}
}
