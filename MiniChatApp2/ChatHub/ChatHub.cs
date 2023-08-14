using Microsoft.AspNetCore.SignalR;

namespace MiniChatApp2.ChatHub
{
    public class ChatHubs :Hub
    {
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;
           
            await Clients.Client(senderId).SendAsync("ReceiveOne", message);
        }
    }
}
