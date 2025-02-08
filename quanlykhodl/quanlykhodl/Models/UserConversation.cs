using quanlykhodl.Common;
using Vonage.Users;

namespace quanlykhodl.Models
{
    public class UserConversation : BaseEntity
    {
        public int userconversationid { get; set; }
        public int userid { get; set; }
        public int conversationid { get; set; }
        public bool isdeleted { get; set; } = false;

        // Quan hệ
        public accounts? User { get; set; }
        public Conversation? Conversation { get; set; }
    }
}
