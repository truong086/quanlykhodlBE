using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Warehousetransferstatus : BaseEntity
	{
		public int? plan { get; set; }
		public Plan? plan_id;
		//public Planwarehouse? plan_id;
		public string? status { get; set; }
		public ICollection<StatusItem>? statusItems { get; set; }
	}
}
