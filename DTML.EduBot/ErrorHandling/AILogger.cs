using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Build.Framework;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace DTML.EduBot.ErrorHandling
{
    internal class EduBotAILogger : ILogger
    {
        private readonly TelemetryClient botTelemetryClient = new TelemetryClient();

        public LoggerVerbosity Verbosity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Parameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void logVerboseTrace(string className, string methodName, TimeSpan eventTime, string logAdditionalDetails)
        {
            var telemetry = new TraceTelemetry("Trace bot calls", SeverityLevel.Verbose);
            telemetry.Properties.Add("className", className);
            telemetry.Properties.Add("methodName", methodName);
            telemetry.Properties.Add("eventTime", eventTime.ToString());

            if (!string.IsNullOrWhiteSpace(logAdditionalDetails))
                telemetry.Properties.Add("AdditionalDetails", logAdditionalDetails);

            botTelemetryClient.TrackTrace(telemetry);
        }

        public void logExceptions(Exception exMessage, string methodName)
        {
            var telemetry = new TraceTelemetry("Log bot exceptions", SeverityLevel.Verbose);

            var exTime = new Dictionary<string, string>
            { {"exceptionTime", DateTime.Now.ToShortDateString()}};


            botTelemetryClient.TrackException(exMessage, exTime);
        }

        public void Initialize(IEventSource eventSource)
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}