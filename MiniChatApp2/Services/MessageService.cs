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

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<MessageResponseDto> SendMessageAsync(MessageCreateDto message, int senderId)
        {
            // Additional validation and business logic can be implemented here

            var messageResponse = await _messageRepository.SaveMessageAsync(message,senderId);

            return messageResponse;
        }
    }
}

