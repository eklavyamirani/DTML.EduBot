using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DTML.EduBot.Common;

namespace DTML.EduBot.Dialogs
{
    [LuisModel("31511772-4f1c-4590-87a8-0d6b8a7707a1", "a88bd2b022e34d5db56a73eb2bd33726")]

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