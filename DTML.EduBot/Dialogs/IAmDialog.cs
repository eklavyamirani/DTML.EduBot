namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;
    using DTML.EduBot.Constants;
    using DTML.EduBot.Extensions;

    public partial class ChitChatDialog : QnaLuisDialog <Object>
    {
        [LuisIntent("UserEmotion")]
        public async Task HandleIAmIntent(IDialogContext context, LuisResult result)
        {
           await context.PostTranslatedAsync(BotPersonality.GetRandomGenericResponse());
        }
    }
}