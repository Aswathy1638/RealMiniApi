using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface ILogRepository
    {

        Task<IEnumerable<object>> GetLogs(DateTime? customStartTime, DateTime? customEndTime);
    }
}
