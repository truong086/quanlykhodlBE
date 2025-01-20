using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Retailcustomers : BaseEntity
	{
		public string? name { get; set; }
		public string? address { get; set; }
		public string? phone { get; set; }
		public string? email { get; set; }
        public ICollection<Deliverynote>? deliverynotes { get; set; }
    }
}
