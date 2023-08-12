using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.Globalization;

namespace MiniChatApp2.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly RealAppContext _context;
        private readonly List<Logs> _logs;


        public LogRepository(RealAppContext context)
        {
            _context = context;
            _logs = new List<Logs>();
        }
        public async Task<IEnumerable<Logs>> GetLogsByTimestampAsync(DateTime startTime, DateTime endTime)
        {

            Console.WriteLine("repo: " + startTime);
            var allLogs = await _context.Logs.ToListAsync();
            

            var filteredLogs = allLogs
                 .Where(log => ParseTimeStamp(log.TimeStamp) >= startTime && ParseTimeStamp(log.TimeStamp) <= endTime)
        //.Where(log => DateTime.ParseExact(log.TimeStamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) >= startTime &&
        //              DateTime.ParseExact(log.TimeStamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) <= endTime)
        .Select(log => new Logs
        {
            id = log.id,
            ip = log.ip,
            Username = log.Username,
            RequestBody = log.RequestBody.Replace("\n", "").Replace("\"", "").Replace("\r", ""),
            TimeStamp = log.TimeStamp,
        });

            return filteredLogs;
        }

        private DateTime ParseTimeStamp(string timeStamp)
        {
            DateTime result;
            if (DateTime.TryParseExact(timeStamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return DateTime.MinValue; // or handle parsing failure as needed
        }


    }
}

