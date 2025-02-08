using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class ImageProduct : BaseEntity
	{
		public int? productmap { get; set; }
		// Thuộc tính điều hướng
		public product? products_id { get; set; }
		public string? public_id { get; set; }
		public string? link { get; set; }
	}
}
