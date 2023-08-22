using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
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

            //var authorizationHeader = Context.GetHttpContext().Request.Headers["Authorization"];
            //Console.WriteLine("Authorization Header: " + authorizationHeader);
            var userId = GetCurrentUserId();
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"CID: {connectionId}, UID: {userId}");

            // Associate user ID with connection ID
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(connectionId))
            {
                _userConnectionManager.AddConnection(userId, connectionId);
                await Groups.AddToGroupAsync(connectionId, userId);

            }

            await base.OnConnectedAsync();
        }
        public async Task SendMessage(MessageResponseDto message,string senderId)
        {
            // Console.WriteLine(receiverId + "  is rec id " + message);
            //var senderId = Context.UserIdentifier;


            //Console.WriteLine("RID BND",senderId);
            //await Clients.Client(receiverId).SendAsync("ReceiveOne",senderId, message);
            //await _messageService.SendMessageToSender(senderId, message);
            //  if (userConnectionMap.TryGetValue(receiverId, out var connectionId))

            //{

            //var currentUser = HttpContext.User;
            //var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var userId = GetCurrentUserId();
            var receiverId = message.ReceiverId; // Check the value here
            Console.WriteLine($"UserId: {senderId}, ReceiverId: {receiverId}");
            
          //  var messageResponse = await _messageService.SendMessageAsync(message,senderId);
           // var connectionId = await GetConnectionId(receiverId);


            //if (connectionId != null)
            //{
                var newmessageResponse = await _messageService.SendMessageAsync(message, senderId);
               // await Clients.Client(connectionId).SendAsync("ReceiveOne", newmessageResponse);
                await Clients.All.SendAsync("ReceiveOne", newmessageResponse,senderId);

            //}

            Console.WriteLine("Completed");
            // await Clients.Client(connectionId).SendAsync("ReceiveOne", message);
            //}

        }

        public async Task EditMessage(int messageId,MessageEditDto editedMessage, string editorId)
        {
            // Process edited message and notify clients
           
            var edit = _messageService. EditMessageAsync(messageId,editedMessage, editorId);
            await Clients.All.SendAsync("MessageEdited", editedMessage, editorId);
        }


        //private string GetReceiverConnectionId(string receiverId)
        //{
        //    if (_userConnectionManager.TryGetConnectionId(receiverId, out var connectionId))

        //    {
        //        return connectionId;
        //    }
        //    return null;
        //}
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
    //private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();

    //public override async Task OnConnectedAsync()
    //{
    //    var userId = Context.UserIdentifier;
    //    Console.WriteLine(userId);

    //    var connectionId = Context.ConnectionId;

    //    ConnectedUsers.TryAdd(userId, connectionId);

    //    await base.OnConnectedAsync();
    //}

    //public override async Task OnDisconnectedAsync(Exception exception)
    //{
    //    var userId = Context.UserIdentifier;
    //    Console.WriteLine(userId);

    //    ConnectedUsers.TryRemove(userId, out _);

    //    await base.OnDisconnectedAsync(exception);
    //}

    //public async Task SendMessage(string receiverId, string message)
    //{
    //    var senderId = Context.UserIdentifier;

    //    if (ConnectedUsers.TryGetValue(receiverId, out string connectionId))
    //    {
    //        await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message);
    //    }

    //}
}





