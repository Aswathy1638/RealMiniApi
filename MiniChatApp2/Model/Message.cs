using Microsoft.AspNetCore.Identity;

namespace MiniChatApp2.Model
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string receiverId { get; set; }
        public string senderId { get; set; }
        
        public IdentityUser sender { get; set; }
        public IdentityUser receiver { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
