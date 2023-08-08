/*
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using static MiniChatApp2.Model.MessageResponseDto;
{
namespace MiniChatApp2.Controllers
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly RealAppContext _context;
        private static readonly List<Message> _messages = new List<Message>();

        public MessagesController(IMessageService messageService, RealAppContext context)
        {
            _messageService = messageService;
            _context = context;
        }




        // GET: api/Messages
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetConversationHistory(int userId, DateTime? before, int count = 20, string sort = "asc")
        {
            var result = await _messageService.GetConversationHistoryAsync(userId, before, count, sort);

            return result;
        }

        // GET: api/Messages/5



        [HttpGet("{id}")]

        public async Task<IActionResult> GetMessage(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request parameter." });
            }

            var currentId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserId = Convert.ToInt32(currentId);

            // Fetch messages sent by the current user
            var sentMessages = await _context.Message
                .Where(u => u.senderId == currentUserId && u.receiverId == id)
                .Select(u => new
                {
                    id = u.Id,
                    senderId = u.senderId,
                    receiverId = u.receiverId,
                    content = u.Content,
                    timestamp = u.Timestamp
                })
                .ToListAsync();

            // Fetch messages received by the current user
            var receivedMessages = await _context.Message
                .Where(u => u.senderId == id && u.receiverId == currentUserId)
                .Select(u => new
                {
                    id = u.Id,
                    senderId = u.senderId,
                    receiverId = u.receiverId,
                    content = u.Content,
                    timestamp = u.Timestamp
                })
                .ToListAsync();

            // Combine sent and received messages into a single list
            var messages = sentMessages.Union(receivedMessages).ToList();

            if (messages == null || messages.Count == 0)
            {
                return NotFound(new { message = "User or conversation not found" });
            }

            return Ok(messages);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, MessageEditDto message)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messages = await _context.Message.FirstOrDefaultAsync(u => u.Id == id);

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            if (Convert.ToInt32(userId) != messages.senderId)
            {
                return Unauthorized(new { message = "Unauthorized access" });
            }



            var editedMessage = await _messageService.EditMessageAsync(id, message, int.Parse(userId));

            if (editedMessage == null)
            {
                return NotFound(new { error = "Message not found or not editable by the current user." });
            }

            return Ok(new { message = "Message edited successfully." });
        }
    

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MessageResponseDto>> PostMessage(MessageCreateDto message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Message sending failed due to validation errors." });
            }
            var currentUser = HttpContext.User;
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Unauthorized access." });
            }
            var messageResponse = await _messageService.SendMessageAsync(message, int.Parse(userId));

            if (messageResponse == null)
            {
                return NotFound(new { error = "Sender or receiver user not found." });
            }

            return Ok(messageResponse);
        }



        // DELETE: api/Messages/5
        /*   [HttpDelete("{id}")]

           public async Task<IActionResult> DeleteMessage(int id)
           {
               var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

               // Check if the message exists
               var message = await _context.Message.FindAsync(id);
               if (message == null)
               {
                   return NotFound(new { error = "Message not found" });
               }

               // Check if the current user is the sender of the message
               if (Convert.ToInt32(userId) != message.senderId)
               {
                   return Unauthorized(new { error = "You are not authorized to delete this message" });
               }

               // Delete the message and save changes
               _context.Message.Remove(message);
               await _context.SaveChangesAsync();

               return Ok(new { message = "Message deleted successfully" });
           }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var result = await _messageService.DeleteMessageAsync(id);

            return result;
        }

        private bool MessageExists(int id)
        {
            return (_context.Message?.Any(e => e.Id == id)).GetValueOrDefault();
        }
}
    }*/
        
