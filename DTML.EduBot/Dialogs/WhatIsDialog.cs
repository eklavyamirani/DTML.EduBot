using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DTML.EduBot.Constants;
using DTML.EduBot.Common;

namespace DTML.EduBot.Dialogs
{
    public partial class ChitChatDialog : QnaLuisDialog<object>
    {

        [LuisIntent("BotQuestions")]
        public async Task HandleWhatIsIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Name))
            {
                await context.PostAsync($"My name is {BotPersonality.BotName}");
            }
            else
            {
                await context.PostAsync(BotPersonality.GetRandomGenericResponse());
            }
        }
    }
}