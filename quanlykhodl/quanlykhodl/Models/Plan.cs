using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Plan : BaseEntity
	{
		public string? title { get; set; }
		public string? description { get; set; }
		public string? status { get; set; }
		public string? localtionnew { get; set; }
        public bool isConfirmation { get; set; }
        public bool isConsent { get; set; }
        public int? product_map { get; set; }
		public int? Receiver { get; set; }
		public int? warehouse { get; set; }
		public int? area { get; set; }
		public int? floor { get; set; }
		public product? productid123 { get; set; }
		public Account? Receiver_id { get; set; }
		public Warehouse? warehouse_id { get; set; }
		public Area? areaid { get; set; }
		public Floor? floor_id { get; set; }
		public virtual ICollection<Warehousetransferstatus>? warehousetransferstatuses { get; set; }
	}
}
