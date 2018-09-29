using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static DTML.EduBot.Common.AzureTableLogger;

namespace DTML.EduBot.Common.Interfaces
{
    public interface ILogger
    {
        Task Log(string userId, string message, string eventType);

        Task Log(LogEntry log);

        Task Log(Exception exception);
    }
}