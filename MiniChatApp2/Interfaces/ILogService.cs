using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface ILogService
    {
        Task<List<Logs>> GetLogsAsync(DateTime startTime, DateTime endTime);
    }
}