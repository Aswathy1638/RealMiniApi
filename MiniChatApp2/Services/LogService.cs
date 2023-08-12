using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using MiniChatApp2.Repositories;

namespace MiniChatApp2.Services
{
    public class LogService :ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<List<Logs>> GetLogsAsync(DateTime startTime, DateTime endTime)
        {
            return (await _logRepository.GetLogsByTimestampAsync(startTime, endTime)).ToList();
        }
    }
}
