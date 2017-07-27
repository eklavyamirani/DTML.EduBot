using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace DTML.EduBot.Dialogs
{
    public class HelpDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Sure, happy to help! Here are a few commands:");
            await context.PostAsync($"1. Send message to mentor {{mentor name}}");
            await context.PostAsync($"2. Quit the lesson. (if you are taking one.)");
            await context.PostAsync($"3. Translate {{phrase}} to {{language}}");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var helpActivity = await result;
            if (helpActivity == null)
            {
                await this.StartAsync(context);
                return;
            }

            var helptext = helpActivity.Text;
            
            //TODO: implement help
            if (helptext.Equals("Test", StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync("TODO. To be implemented");
                
            }
            else
            {
                await context.PostAsync(Constants.Shared.CannotHelpMessage);
            }

            context.Wait(ResumePreviousDialog);
        }

        private async Task ResumePreviousDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var nextActivity = await result;
            context.Done(nextActivity);
        }
    }
}