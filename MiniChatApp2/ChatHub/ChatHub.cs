using Microsoft.AspNetCore.SignalR;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.Security.Claims;

namespace MiniChatApp2.ChatHub
{
    public class ChatHubs :Hub
    {
        private readonly IMessageService _messageService;
        private Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHubs(IMessageService messageService, IHttpContextAccessor httpContextAccessor)
        {
            _messageService = messageService;

            _httpContextAccessor = httpContextAccessor;

        }
        public override async Task OnConnectedAsync()
        {

             var authorizationHeader = Context.GetHttpContext().Request.Headers["Authorization"];
            Console.WriteLine("Authorization Header: " + authorizationHeader);
            var userId = GetCurrentUserId();
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"CID: {connectionId}, UID: {userId}");

            // Associate user ID with connection ID
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(connectionId))
            {
                userConnectionMap[userId] = connectionId;
                await Groups.AddToGroupAsync(connectionId, userId);
              
            }

            await base.OnConnectedAsync();
        }
        public async Task SendMessage(Message message)
        {
            // Console.WriteLine(receiverId + "  is rec id " + message);
            //var senderId = Context.UserIdentifier;


            //Console.WriteLine("RID BND",senderId);
            //await Clients.Client(receiverId).SendAsync("ReceiveOne",senderId, message);
            //await _messageService.SendMessageToSender(senderId, message);
            //  if (userConnectionMap.TryGetValue(receiverId, out var connectionId))

            //{
           // var userId = GetCurrentUserId();
            //var connectionId = Context.ConnectionId;

            await Clients.All.SendAsync("ReceiveOne", message);
            // await Clients.Client(connectionId).SendAsync("ReceiveOne", message);
            //}

        }
        private string GetCurrentUserId()
        {
            // Retrieve the user ID from the ClaimsPrincipal (User) available in the controller
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}
