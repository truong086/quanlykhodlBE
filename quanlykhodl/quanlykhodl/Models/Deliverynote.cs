using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Deliverynote : BaseEntity
	{
		public string? title { get; set; }
		public string? description { get; set; }
        public int? retailcustomers { get; set; }
        public Retailcustomers? retailcustomers_id { get; set; }
        public int? accountmap { get; set; }
		public accounts? account { get; set; }
		public bool isRetailcustomers { get; set; }
		public double price { get; set; }
		public int quantity { get; set; }
		public double total { get; set; }
		public string? deliveryaddress { get; set; }
		public string? code { get; set; }
        public int? tax { get; set; }
        public bool? ispercentage { get; set; }
        public bool? isaction { get; set; }
        public bool ispack { get; set; }
		public ICollection<productDeliverynote>? productDeliverynotes { get; set; }
		//public ICollection<DeliverynotePrepareToExport>? deliverynotePrepareToExports { get; set; }
	}
}
