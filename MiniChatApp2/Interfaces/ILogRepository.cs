﻿using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface ILogRepository
    {
        Task<IEnumerable<Logs>> GetLogsByTimestampAsync(DateTime startTime, DateTime endTime);
    }
}