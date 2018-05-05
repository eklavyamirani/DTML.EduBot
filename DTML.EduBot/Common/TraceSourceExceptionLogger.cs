using DTML.EduBot.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using static DTML.EduBot.Common.AzureTableLogger;

namespace DTML.EduBot.Common
{
    public class TraceSourceExceptionLogger : ExceptionLogger
    {
        private readonly ILogger _logger;
        public TraceSourceExceptionLogger(ILogger logger)
        {
            _logger = logger;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Log(new LogEntry { message = "TraceSourceExceptionLogger: "+context.Exception.ToString(), date = DateTime.Now.ToShortDateString(), eventType = "Exception" });
        }
    }
}