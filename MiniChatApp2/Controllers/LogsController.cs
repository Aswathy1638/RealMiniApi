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
        private readonly RealAppContext _context;
        private readonly ILogService _logService;
        public LogsController(RealAppContext context,ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime? startTime = null, [FromQuery] DateTime? endTime = null)
        {

            if (startTime == null)
                startTime = DateTime.Now.AddMinutes(-5);

            Console.WriteLine(startTime);

            if (endTime == null)
                endTime = DateTime.Now;
            Console.WriteLine(endTime);

            if (startTime > endTime)
            {
                return BadRequest(new { error = "StartTime must be earlier than EndTime" });
            }

            List<Logs> logs = await _logService.GetLogsAsync(startTime.Value, endTime.Value);

            if (logs.Count == 0)
            {
                return NotFound(new { message = "Logs not found" });
            }

            return Ok(new { Logs = logs });
        }
    }
}