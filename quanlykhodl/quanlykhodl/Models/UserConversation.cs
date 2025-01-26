using quanlykhodl.Common;
using Vonage.Users;

namespace quanlykhodl.Models
{
    public class UserConversation : BaseEntity
    {
        public int UserConversationId { get; set; }
        public int UserId { get; set; }
        public int ConversationId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Quan hệ
        public Account? User { get; set; }
        public Conversation? Conversation { get; set; }
    }
}
