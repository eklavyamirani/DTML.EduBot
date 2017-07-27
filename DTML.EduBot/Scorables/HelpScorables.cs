using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using DTML.EduBot.Extensions;
using DTML.EduBot.Dialogs;

namespace DTML.EduBot.Scorables
{
    public class HelpScorables : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask _dialogTask;

        public HelpScorables(IDialogTask dialogTask)
        {
            Set.FieldNotNull(dialogTask, nameof(dialogTask), out _dialogTask);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != string.Empty;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var activity = item as IMessageActivity;
            if (activity == null || string.IsNullOrWhiteSpace(activity.Text))
            {
                return;
            }

            if (Constants.Shared.HelpCommand.Equals(activity.Text, StringComparison.InvariantCultureIgnoreCase))
            {
                var helpDialog = new HelpDialog();
                // We really dont want to do anything after the user is done with the help
                _dialogTask.Call(helpDialog, null);
                await this._dialogTask.PollAsync(token);
            }
        }

        protected override Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var activity = item as IMessageActivity;
            if (activity == null || string.IsNullOrWhiteSpace(activity.Text))
            {
                return Task.FromResult(string.Empty);
            }

            // TODO: Test using Intent?
            // TODO: Translation
            if (Constants.Shared.HelpCommand.Equals(activity.Text, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(Constants.Shared.HelpCommand);
            }

            return Task.FromResult(string.Empty);
        }
    }
}