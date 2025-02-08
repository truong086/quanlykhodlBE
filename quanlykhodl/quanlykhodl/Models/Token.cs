using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Token : BaseEntity
	{
		public int? account_id { get; set; }
		public accounts? account { get; set; }
		public string? code { get; set; }
        public string? status { get; set; }
    }
}
