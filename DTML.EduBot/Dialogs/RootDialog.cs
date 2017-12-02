namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.Common;
    using DTML.EduBot.Constants;
    using DTML.EduBot.UserData;
    using Extensions;

    [Serializable]
    public class RootDialog : IDialog<string>
    {
        private readonly ChitChatDialog _chitChatDialog;
        private readonly LevelDialog _levelDialog;
        private readonly IUserDataRepository _userDataRepository;

        public RootDialog(ChitChatDialog chitChatDialog, LevelDialog _levelDialog, IUserDataRepository userDataRepository)
        {
            this._chitChatDialog = chitChatDialog;
            this._userDataRepository = userDataRepository;
            this._levelDialog = _levelDialog;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity messageActivity;

            try
            {
                messageActivity = (await result);
            }
            catch (Exception)
            {
                return;
            }

                await context.Forward(_chitChatDialog, this.AfterChitChatComplete, messageActivity, CancellationToken.None);
        }
                
        private async Task AfterChitChatComplete(IDialogContext context, IAwaitable<object> result)
        {
            string userName = context.UserData.ContainsKey(Constants.Shared.UserName) ? context.UserData.GetValue<string>(Constants.Shared.UserName) : string.Empty;
            await Task.WhenAll(context.PostTranslatedAsync($"OK, {userName}. What do you want to do now? Type 'learn english' to start learning lessons or ask me any question"), this.StartAsync(context));
        }

        private async Task AfterDialogEnded(IDialogContext context, IAwaitable<object> result)
        {
            await AfterChitChatComplete(context, result);
        }

       
    }
}