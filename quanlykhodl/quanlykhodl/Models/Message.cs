using quanlykhodl.Common;
using Vonage.Users;

namespace quanlykhodl.Models
{
    public class Message : BaseEntity
    {
        public int? senderid { get; set; } // Id người gửi
        public int? receiverid { get; set; } // Id người nhận
        public string content { get; set; } = string.Empty; // Nội dung tin nhắn
        public string? image { get; set; }
        public string? publicid { get; set; }
        public bool isread { get; set; } = false; // Đã đọc hay chưa

        // Quan hệ
        public accounts? Sender { get; set; }
        public accounts? Receiver { get; set; }
    }
}
