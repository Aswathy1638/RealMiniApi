﻿using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.Linq.Expressions;

namespace MiniChatApp2.Repositories
{
    public class MessageRepository:IMessageRepository
    {
        private readonly MiniChatApp2Context _dbContext;

        public MessageRepository(MiniChatApp2Context context)
        {
            _dbContext = context;
        }

        public async Task<MessageResponseDto> SaveMessageAsync(MessageCreateDto message, int senderId)
        {
            // Check if the sender user exists
            var senderUser = await _dbContext.User.FindAsync(senderId);
            if (senderUser == null)
            {
                return null;
            }

            // Check if the receiver user exists
            var receiverUser = await _dbContext.User.FindAsync(message.receiverId);
            if (receiverUser == null)
            {
                return null;
            }

            var messageEntity = new Message
            {
                senderId =senderId,
                receiverId = message.receiverId,
                Content = message.Content,
                Timestamp = DateTime.Now
            };

            _dbContext.Message.Add(messageEntity);
            await _dbContext.SaveChangesAsync();

            var messageResponse = new MessageResponseDto
            {
                MessageId = messageEntity.Id,
                SenderId = messageEntity.senderId,
                ReceiverId = messageEntity.receiverId,
                Content = messageEntity.Content,
                Timestamp = messageEntity.Timestamp,
            };

            return messageResponse;
        }

        public async Task<MessageResponseDto> EditMessageAsync(int messageId, MessageEditDto message, int editorId)
        {
            var existingMessage = await _dbContext.Message.FindAsync(messageId);
            if (existingMessage == null)
            {
                return null;
            }

            // Check if the current user is the sender of the message
            if (existingMessage.senderId != editorId)
            {
                return null;
            }

            existingMessage.Content = message.Content;
            _dbContext.Message.Update(existingMessage);
            await _dbContext.SaveChangesAsync();

            var messageResponse = new MessageResponseDto
            {
                MessageId = existingMessage.Id,
                SenderId = existingMessage.senderId,
                ReceiverId = existingMessage.receiverId,
                Content = existingMessage.Content,
                Timestamp = existingMessage.Timestamp,
            };

            return messageResponse;
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _dbContext.Message.FindAsync(messageId);
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            var message = await GetMessageByIdAsync(messageId);
            if (message != null)
            {
                _dbContext.Message.Remove(message);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Message>> GetConversationHistoryAsync(int userId, DateTime? before, int count, string sort)
        {
            // Retrieve conversation history based on the provided parameters
            var query = _dbContext.Message
                .Where(m => (m.senderId == userId || m.receiverId == userId));

            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp < before);
            }

            query = sort == "desc" ? query.OrderByDescending(m => m.Timestamp) : query.OrderBy(m => m.Timestamp);

            if (count > 0)
            {
                query = query.Take(count);
            }

            return await query.ToListAsync();
        }

    }
}
