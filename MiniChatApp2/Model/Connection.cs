using System.Collections.Concurrent;

namespace MiniChatApp2.Model
{
    public class Connections
    {
        private readonly ConcurrentDictionary<string, string> userConnectionMap = new ConcurrentDictionary<string, string>();

        public void AddConnection(string userId, string connectionId)
        {
            userConnectionMap[userId] = connectionId;
        }

        public Task<string> GetConnectionIdAsync(string userId)
        {
            if (userConnectionMap.TryGetValue(userId, out var connectionId))
            {
                return Task.FromResult(connectionId);
            }
            else
            {
                return Task.FromResult<string>(null);
            }

        }
    }
}
