using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using MiniChatApp2.Services;
using NuGet.Protocol.Plugins;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace MiniChatApp2.ChatHub
{
    public class ChatHubs : Hub
    {


        private readonly Connections _userConnectionManager;

        private readonly IMessageService _messageService;
        private Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IHubContext<ChatHubs> HubContext { get; }

        public ChatHubs(IMessageService messageService, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHubs> hubContext, Connections userConnectionManager)
        {
            _messageService = messageService;

            _httpContextAccessor = httpContextAccessor;
            HubContext = hubContext;
            _userConnectionManager = userConnectionManager;
        }
        public override async Task OnConnectedAsync()
        {
                       
            var userId = GetCurrentUserId();
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"CID: {connectionId}");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(connectionId))
            {
                _userConnectionManager.AddConnection(userId, connectionId);
                await Groups.AddToGroupAsync(connectionId, userId);

            }

            await base.OnConnectedAsync();
        }
        public async Task SendMessage(MessageResponseDto message,string senderId)
        {
            
            var receiverId = message.ReceiverId; 
            Console.WriteLine($"UserId: {senderId}, ReceiverId: {receiverId}");
                   
                var newmessageResponse = await _messageService.SendMessageAsync(message, senderId);
         
                await Clients.All.SendAsync("ReceiveOne", newmessageResponse,senderId);

                Console.WriteLine("Completed");
         
        }

        public async Task EditMessage(int messageId,MessageEditDto editedMessage, string editorId)
        {
           
           
            var edit = _messageService. EditMessageAsync(messageId,editedMessage, editorId);
            await Clients.All.SendAsync("MessageEdited", editedMessage, editorId);
        }
        public async Task DeleteMessage(int messageId)
        {
            var edit = _messageService.DeleteMessageAsync(messageId);
            await Clients.All.SendAsync("MessageDeleted", messageId);
        }
        private string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }


        public void AddConnection(string userId, string connectionId)
        {
            userConnectionMap[userId] = connectionId;
        }

        private async Task<string> GetConnectionId(string userId)
        {
            return await _userConnectionManager.GetConnectionIdAsync(userId);
        }


    }
    
}





