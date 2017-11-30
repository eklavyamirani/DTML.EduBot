using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using DTML.EduBot.Common;
using DTML.EduBot.Extensions;
using DTML.EduBot.Constants;


namespace DTML.EduBot.Dialogs
{
    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {

            if (result.Entities.Any(e => e.Type == BotEntities.StartConversation))
            {
                string botresponse = BotPersonality.GetRandomGreeting();
                await context.PostAsync(botresponse);
            }
            else if (result.Entities.Any(e => e.Type == BotEntities.EndConversation))
            {
                string botresponse = BotPersonality.GetRandomGoodbye();
                await context.PostAsync(botresponse);
            }
            else {
                await context.PostAsync(":)");
            }
        }

        [LuisIntent("Courtesy")]
        public async Task Courtesy(IDialogContext context, LuisResult result)
        {
            await context.PostTranslatedAsync(BotPersonality.GetRandomGenericResponse());
            await EngageWithUser(context);
        }
    }
}