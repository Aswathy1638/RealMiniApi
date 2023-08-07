using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface ILogService
    {
        Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime);
    }
}
