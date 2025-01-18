using quanlykhodl.Common;
using System.Numerics;

namespace quanlykhodl.Models
{
	public class Notification
	{
		public int? plan_map { get; set; }
		//public Plan? plan_id { get; set; }
		public bool isConfirmation { get; set; }
		public bool isConsent { get; set; }
	}
}
