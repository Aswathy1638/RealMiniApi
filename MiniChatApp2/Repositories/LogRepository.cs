using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;

namespace MiniChatApp2.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly MiniChatApp2Context _context;



        public LogRepository(MiniChatApp2Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Logs>> GetAllLogsAsync()
        {
            return await _context.Logs.ToListAsync();
        }
        public async Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime)
        {
            IEnumerable<Logs> logs;

            if (customStartTime.HasValue && customEndTime.HasValue)
            {
                logs = _context.Logs
                    .Where(log => DateTime.Parse(log.TimeStamp) >= customStartTime.Value && DateTime.Parse(log.TimeStamp) <= customEndTime.Value);
            }
            else
            {
                logs = _context.Logs;
            }

            return logs
                .Select(u => new
                {
                    Id = u.id,
                    Ip = u.ip,
                    Username = u.Username,
                    RequestBody = u.RequestBody.Replace("\n", "").Replace("\"", "").Replace("\r", ""),
                    TimeStamp = DateTime.Parse(u.TimeStamp), // Convert string to DateTime if necessary
                });
        }

    }
}
   
