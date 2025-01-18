using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Token : BaseEntity
	{
		public int? account_id { get; set; }
		public Account? account { get; set; }
		public string? code { get; set; }
        public string? Status { get; set; }
    }
}
