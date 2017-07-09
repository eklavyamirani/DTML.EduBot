using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DTML.EduBot.Common;

namespace DTML.EduBot.Dialogs
{
    [LuisModel("", "")]
    [Serializable]
    public partial class RootDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(BotPersonality.BotResponseUnrecognizedIntent);
        }

       [LuisIntent("Gibberish")]
        public async Task HandleGibberish(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(BotPersonality.BotResponseToGibberish);
        }

       
    }
}