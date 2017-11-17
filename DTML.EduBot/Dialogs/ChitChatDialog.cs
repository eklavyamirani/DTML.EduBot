namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Attributes;
    using Common;
    using Extensions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;

    [PreConfiguredLuisModel]
    [PreConfiguredQnaModel]
    [Serializable]
    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            await context.PostTranslatedAsync(BotPersonality.BotResponseUnrecognizedIntent);
        }
    }
}