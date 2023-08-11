using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.Security.Claims;

namespace MiniChatApp2.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MessageService(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MessageResponseDto> SendMessageAsync(MessageCreateDto message, string senderId)
        {
            // Additional validation and business logic can be implemented here

            var messageResponse = await _messageRepository.SaveMessageAsync(message,senderId);

            return messageResponse;
        }

        public async Task<MessageResponseDto> EditMessageAsync(int messageId, MessageEditDto message, string editorId)
        {
            var editedMessage = await _messageRepository.EditMessageAsync(messageId, message, editorId);

            return editedMessage;
        }

        public async Task<IActionResult> DeleteMessageAsync(int messageId)
        {
            // Fetch the message by its ID from the repository asynchronously
            var message = await _messageRepository.GetMessageByIdAsync(messageId);

           // Check if the message exists
            if (message == null)
           {
               return new NotFoundObjectResult(new { error = "Message not found" });
            }

           // Check if the current user is the sender of the message (you need to implement authentication)
           if (GetCurrentUserId() != message.senderId.ToString())
           {
               return new UnauthorizedObjectResult(new { error = "You are not authorized to delete this message" });
           }

           // Delete the message and save changes
          await _messageRepository.DeleteMessageAsync(messageId);

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