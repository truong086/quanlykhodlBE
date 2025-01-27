using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class OnlineUsers : BaseEntity
    {
        public int? account_id { get; set; }
        public Account? account { get; set; }
        public string? ConnectionId { get; set; } // ConnectionId của SignalR
        public bool IsOnline { get; set; }
    }
}
