using MiniChatApp2.Model;
using System.Linq.Expressions;
using static MiniChatApp2.Model.MessageResponseDto;

namespace MiniChatApp2.Interfaces
{
    public interface IMessageRepository
    {
        Task<MessageResponseDto> SaveMessageAsync(MessageCreateDto message, int senderId);
        Task<MessageResponseDto> EditMessageAsync(int id, MessageEditDto message, int editorId);

    }
}
