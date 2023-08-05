using MiniChatApp2.Model;
using System.Linq.Expressions;

namespace MiniChatApp2.Interfaces
{
    public interface IMessageRepository
    {
        Task<MessageResponseDto> SaveMessageAsync(MessageCreateDto message, int senderId);

    }
}
