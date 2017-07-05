using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DTML.EduBot.Common;

namespace DTML.EduBot.Dialogs
{
    public partial class RootDialog : LuisDialog <Object>
    {
        [LuisIntent("IAm")]
        public async Task HandleIAmIntent(IDialogContext context, LuisResult result)
        {
           await context.PostAsync(BotPersonality.GetRandomGenericResponse() + "<br/>" + BotPersonality.GetChatContinuer());
        }
    }
}