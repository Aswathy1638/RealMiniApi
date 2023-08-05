﻿using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendMessageAsync(MessageCreateDto message, int senderId);
    }
}