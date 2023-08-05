using Microsoft.AspNetCore.Mvc;
using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendMessageAsync(MessageCreateDto message, int senderId);
        Task<MessageResponseDto> EditMessageAsync(int messageId, MessageEditDto message, int editorId);
        Task<IActionResult> DeleteMessageAsync(int messageId);
        Task<IActionResult> GetConversationHistoryAsync(int userId, DateTime? before, int count, string sort);

    }
}
