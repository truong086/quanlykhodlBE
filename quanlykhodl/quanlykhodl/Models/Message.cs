using quanlykhodl.Common;
using Vonage.Users;

namespace quanlykhodl.Models
{
    public class Message : BaseEntity
    {
        public int? SenderId { get; set; } // Id người gửi
        public int? ReceiverId { get; set; } // Id người nhận
        public string Content { get; set; } = string.Empty; // Nội dung tin nhắn
        public bool IsRead { get; set; } = false; // Đã đọc hay chưa

        // Quan hệ
        public Account? Sender { get; set; }
        public Account? Receiver { get; set; }
    }
}
