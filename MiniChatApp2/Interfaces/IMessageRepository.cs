using MiniChatApp2.Model;
using System.Linq.Expressions;
using static MiniChatApp2.Model.MessageResponseDto;

namespace MiniChatApp2.Interfaces
{
    public interface IMessageRepository
    {
        Task<MessageResponseDto> SaveMessageAsync(MessageCreateDto message, string senderId);
       Task<MessageResponseDto> EditMessageAsync(int id, MessageEditDto message, string editorId);
      //  Task<Message> GetMessageByIdAsync(int messageId);
       // Task DeleteMessageAsync(int messageId);
       // Task<List<Message>> GetConversationHistoryAsync(int userId, DateTime? before, int count, string sort);
    }
}
