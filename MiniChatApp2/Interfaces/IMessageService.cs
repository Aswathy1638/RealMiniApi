using Microsoft.AspNetCore.Mvc;
using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendMessageAsync(MessageCreateDto message, string senderId);
        Task<MessageResponseDto> EditMessageAsync(int messageId, MessageEditDto message, string editorId);
        Task<IActionResult> DeleteMessageAsync(int messageId);
        Task<List<MessageResponseDto>> SearchConversationsAsync(string userId, string query);
        Task SendMessageToSender(string senderId, string message);

        //  Task<IActionResult> GetConversationHistoryAsync(string userId, DateTime? before, int count, string sort);
        Task<List<Message>> GetConversationHistoryAsync(string receiver, DateTime? before, int count = 20, string sort = "asc");
        //Task<List<MessageDto>> GetConversationHistoryAsync(int currentUserId, int otherUserId, DateTime? before, int count, string sort);

    }
}
