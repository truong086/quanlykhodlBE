using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class OnlineUsers : BaseEntity
    {
        public int? account_id { get; set; }
        public accounts? account { get; set; }
        public string? connectionid { get; set; } // connectionid của SignalR
        public bool isonline { get; set; }
    }
}
