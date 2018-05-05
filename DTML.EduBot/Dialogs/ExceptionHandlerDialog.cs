using DTML.EduBot.Common.Interfaces;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static DTML.EduBot.Common.AzureTableLogger;

namespace DTML.EduBot.Dialogs
{
    [Serializable]
    public class ExceptionHandlerDialog<T> : IDialog<object>
    {
        private readonly IDialog<T> _dialog;
        private ILogger _logger;

        public ExceptionHandlerDialog(IDialog<T> dialog, ILogger logger)
        {
            _dialog = dialog;
            _logger = logger;
        }

        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                context.Call(_dialog, ResumeAsync);
            }
            catch (Exception e)
            {
              await LogException(context, e).ConfigureAwait(false);
            }
        }

        private async Task ResumeAsync(IDialogContext context, IAwaitable<T> result)
        {
            try
            {
                context.Done(await result);
            }
            catch (Exception e)
            {
               await LogException(context, e).ConfigureAwait(false);
            }
        }

        private async Task LogException(IDialogContext context, Exception e)
        {
            
            var stackTrace = e.StackTrace;
            if (stackTrace.Length > _stackTraceLength)
                stackTrace = stackTrace.Substring(0, _stackTraceLength) + "…";
            stackTrace = stackTrace.Replace(Environment.NewLine, "  \n");

            var message = e.Message.Replace(Environment.NewLine, "  \n");

            var exceptionStr = $"**{message}**  \n\n{stackTrace}";

            await _logger.Log(new LogEntry { message = exceptionStr, date = DateTime.Now.ToShortDateString(), eventType = "Exception" });
        }
    }
}