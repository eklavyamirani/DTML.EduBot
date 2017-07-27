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
                string translatedBotResponse = await this.TranslateBotResponseAsync(context, $"My name is {BotPersonality.BotName}");

                await context.PostAsync(translatedBotResponse);
            }
            else
            {
                string translatedBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.GetRandomGenericResponse());

                await context.PostAsync(translatedBotResponse);
            }
        }
    }
}