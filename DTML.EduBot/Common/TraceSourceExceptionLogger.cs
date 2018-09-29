using DTML.EduBot.Common.Interfaces;
using System;
using System.Web.Http.ExceptionHandling;

namespace DTML.EduBot.Common
{
    public class TraceSourceExceptionLogger : ExceptionLogger
    {
        public TraceSourceExceptionLogger()
        {
        }

        public override void Log(ExceptionLoggerContext context)
        {
            ILogger _logger = new AzureTableLogger();
            _logger.Log(new LogEntry(Guid.NewGuid().ToString()) { message = "TraceSourceExceptionLogger: "+context.Exception.ToString(), date = DateTime.Now.ToShortDateString(), eventType = "Exception" });
        }
    }
}