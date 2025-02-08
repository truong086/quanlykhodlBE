using quanlykhodl.Common;
using Vonage.Users;

namespace quanlykhodl.Models
{
    public class Conversation : BaseEntity
    {
        public int user1id { get; set; }
        public int user2id { get; set; }
        public int? lastmessageid { get; set; }

        // Quan hệ
        public accounts? User1 { get; set; }
        public accounts? User2 { get; set; }
        public Message? LastMessage { get; set; }
    }
}
