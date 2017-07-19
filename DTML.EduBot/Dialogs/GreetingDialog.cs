using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using DTML.EduBot.Common;

namespace DTML.EduBot.Dialogs
{
    public partial class RootDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await ConvertAndPostResponse(context, BotPersonality.GetRandomGreeting());
        }

        [LuisIntent("Courtesy")]
        public async Task Courtesy(IDialogContext context, LuisResult result)
        {
            await ConvertAndPostResponse(context, BotPersonality.GetRandomGenericResponse());
        }
    }
}