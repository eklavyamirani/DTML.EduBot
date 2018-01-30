using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DTML.EduBot.Dialogs
{
    [Serializable]
    public class ExceptionHandlerDialog<T> : IDialog<object>
    {
        private readonly IDialog<T> _dialog;
        private readonly bool _displayException;
        private readonly int _stackTraceLength;

        public ExceptionHandlerDialog(IDialog<T> dialog, bool displayException, int stackTraceLength = 500)
        {
            _dialog = dialog;
            _displayException = displayException;
            _stackTraceLength = stackTraceLength;
        }

        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                context.Call(_dialog, ResumeAsync);
            }
            catch (Exception e)
            {
                if (_displayException)
                    await DisplayException(context, e).ConfigureAwait(false);
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
                if (_displayException)
                    await DisplayException(context, e).ConfigureAwait(false);
            }
        }

        private async Task DisplayException(IDialogContext context, Exception e)
        {

            var stackTrace = e.StackTrace;
            if (stackTrace.Length > _stackTraceLength)
                stackTrace = stackTrace.Substring(0, _stackTraceLength) + "…";
            stackTrace = stackTrace.Replace(Environment.NewLine, "  \n");

            var message = e.Message.Replace(Environment.NewLine, "  \n");

            var exceptionStr = $"**{message}**  \n\n{stackTrace}";

            await context.PostAsync(exceptionStr).ConfigureAwait(false);
        }
    }
}