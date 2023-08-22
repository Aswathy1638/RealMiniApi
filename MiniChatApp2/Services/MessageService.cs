using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.ChatHub;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.Security.Claims;

namespace MiniChatApp2.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<ChatHubs> _chatHubContext;
        private readonly Connections _userConnectionManager;
        public MessageService(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHubs> chatHubContext, Connections userConnectionManager)
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
            _chatHubContext = chatHubContext;
            _userConnectionManager = userConnectionManager;
        }

        public async Task<MessageResponseDto> SendMessageAsync(MessageResponseDto message, string senderId)
        {
         
            var messageResponse = await _messageRepository.SaveMessageAsync(message,senderId);

            var receiverConnectionId = await _userConnectionManager.GetConnectionIdAsync(message.ReceiverId);
            Console.WriteLine("Hiii");
            Console.WriteLine(receiverConnectionId);
            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await _chatHubContext.Clients.All.SendAsync("ReceiveOne", message,senderId);
            }
            Console.WriteLine("Hiii");
            Console.WriteLine(messageResponse.SenderId);
            Console.WriteLine("Rece");
            Console.WriteLine(messageResponse.ReceiverId);
            Console.WriteLine($"cont");
            Console.WriteLine(messageResponse.Content);
            
            return messageResponse;
        }
        //public async Task SendMessageToSender(string senderId, string message)
        //{

        //    await _chatHubContext.Clients.All.SendAsync("ReceiveOne", message);
        //}
        public async Task<MessageResponseDto> EditMessageAsync(int messageId, MessageEditDto message, string editorId)
        {
            var editedMessage = await _messageRepository.EditMessageAsync(messageId, message, editorId);
            if (editedMessage != null)
            { 

                await _chatHubContext.Clients.All.SendAsync("MessageEdited", editedMessage, editorId);

            }


            return editedMessage;
        }

        public async Task<IActionResult> DeleteMessageAsync(int messageId)
        {
        
            var message = await _messageRepository.GetMessageByIdAsync(messageId);

          
            if (message == null)
           {
               return new NotFoundObjectResult(new { error = "Message not found" });
            }

     
           if (GetCurrentUserId() != message.senderId.ToString())
           {
               return new UnauthorizedObjectResult(new { error = "You are not authorized to delete this message" });
           }

           // Delete the message and save changes
          await _messageRepository.DeleteMessageAsync(messageId);
            await _chatHubContext.Clients.All.SendAsync("MessageDeleted",messageId);

            return new OkObjectResult(new { message = "Message deleted successfully" });
        }

        public async Task<List<Message>> GetConversationHistoryAsync(string receiver, DateTime? before, int count = 20, string sort = "asc")
        {
            var currentUserId= GetCurrentUserId();
            // Fetch the conversation history from the repository asynchronously
            var conversation = await _messageRepository.GetConversationHistoryAsync(currentUserId,receiver, before, count, sort);
            // Check if the conversation exists
            if (conversation == null)
            {
                return null;
            }

            // Return the conversation history
            return conversation;
            //var currentUserId = GetCurrentUserId();


            //var messages = await _messageRepository.GetConversationHistoryAsync(currentUserId, receiver, before, count, sort);

            //if (messages == null || messages.Count == 0)
            //{
            //    return null;
            //}

            //return  messages ;
        }



        public async Task<List<MessageResponseDto>> SearchConversationsAsync(string userId, string query)
        {
            return await _messageRepository.SearchConversationsAsync(userId, query);
        }



        private string GetCurrentUserId()
        {
            // Retrieve the user ID from the ClaimsPrincipal (User) available in the controller
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }






    }
}



//// Fetch the conversation history from the repository asynchronously
//var conversation = await _messageRepository.GetConversationHistoryAsync(userId, before, count, sort);
//// Check if the conversation exists
//if (conversation == null)
//{
//    return new NotFoundObjectResult(new { error = "User or conversation not found" });
//}

//// Return the conversation history
//return new OkObjectResult(new { messages = conversation });
       // public async Task<IActionResult> GetConversationHistoryAsync(string userId, DateTime? before, int count, string sort)
       // {
          
       //}