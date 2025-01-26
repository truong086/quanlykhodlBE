using quanlykhodl.Common;
using Vonage.Users;

namespace quanlykhodl.Models
{
    public class Conversation : BaseEntity
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public int? LastMessageId { get; set; }

        // Quan hệ
        public Account? User1 { get; set; }
        public Account? User2 { get; set; }
        public Message? LastMessage { get; set; }
    }
}
