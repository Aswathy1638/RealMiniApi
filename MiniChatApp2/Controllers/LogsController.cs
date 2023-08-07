using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;

namespace MiniChatApp2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly MiniChatApp2Context _context;
        private readonly ILogService _logService;

        public LogsController(MiniChatApp2Context context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET: api/Logs


        [HttpGet]
        public async Task<IActionResult> GetLogs(DateTime? endTime = null, DateTime? startTime = null)
        {
            var logs = await _logService.GetLogs(endTime, startTime);

            if (!logs.Any())
            {
                return NotFound();
            }

            return Ok(logs);
        }
        

        // GET: api/Logs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Logs>> GetLogs(int id)
        {
          if (_context.Logs == null)
          {
              return NotFound();
          }
            var logs = await _context.Logs.FindAsync(id);

            if (logs == null)
            {
                return NotFound();
            }

            return logs;
        }

        // PUT: api/Logs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogs(int id, Logs logs)
        {
            if (id != logs.id)
            {
                return BadRequest();
            }

            _context.Entry(logs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Logs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Logs>> PostLogs(Logs logs)
        {
          if (_context.Logs == null)
          {
              return Problem("Entity set 'MiniChatApp2Context.Logs'  is null.");
          }
            _context.Logs.Add(logs);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogs", new { id = logs.id }, logs);
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogs(int id)
        {
            if (_context.Logs == null)
            {
                return NotFound();
            }
            var logs = await _context.Logs.FindAsync(id);
            if (logs == null)
            {
                return NotFound();
            }

            _context.Logs.Remove(logs);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogsExists(int id)
        {
            return (_context.Logs?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
