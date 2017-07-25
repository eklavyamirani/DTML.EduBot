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
    public partial class NoneDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string botresponse = BotPersonality.GetRandomGreeting(); 
            await context.PostAsync(botresponse);
        }

        [LuisIntent("Courtesy")]
        public async Task Courtesy(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(BotPersonality.GetRandomGenericResponse());
        }
    }
}