using DTML.EduBot.Common.Interfaces;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DTML.EduBot.Common
{
    public class AzureActivityLogger : IActivityLogger
    {
        private ILogger _logger;

        AzureActivityLogger(ILogger logger)
        {
            _logger = logger;
        }

        public async Task LogAsync(IActivity activity)
        {
            var message = $"From:{activity.From.Id} - To:{activity.Recipient.Id} - Message:{activity.AsMessageActivity()?.Text}";
            await _logger.Log(new LogEntry(Guid.NewGuid().ToString()) { message = message, eventType = activity.Type.ToString() });
        }
    }
}