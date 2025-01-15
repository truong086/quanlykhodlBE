using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class role : BaseEntity
	{
		public string? name { get; set; }

		public ICollection<Account>? account { get; set; }
	}
}
