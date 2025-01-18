using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Account : BaseEntity
	{
		public string? username { get; set; }
		public string? password { get; set; }
		public string? image { get; set; }
		public string? publicid { get; set; }
		public string? email { get; set; }
		public bool Action { get; set; }
		public string? phone { get; set; }
		public string? address { get; set; }
		public int? role_id { get; set; }

		public role? role { get; set; }
		public ICollection<Area>? areas { get; set; }
		public ICollection<product>? products { get; set; }
		public ICollection<category>? categories { get; set; }
		public ICollection<Importform>? importforms { get; set; }
		public ICollection<Deliverynote>? deliverynotes { get; set; }
		public ICollection<Plan>? plans { get; set; }
		public ICollection<Token>? tokens { get; set; }
		public ICollection<Warehouse>? warehouses { get; set; }
		public ICollection<Floor>? floors { get; set; }
		public ICollection<Supplier>? suppliers { get; set; }
	}
}
