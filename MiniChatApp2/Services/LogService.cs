using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;

namespace MiniChatApp2.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime)
        {
            return await _logRepository.GetLogs(customStartTime, customEndTime);
        }
    }
}
