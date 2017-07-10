using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        [LuisIntent("OpenEndedQuestion")]
        public async Task HandleOpenEndedQuestion(IDialogContext context, LuisResult result)
        {
            string botresponse = BotPersonality.GetRandomGenericResponse();
            await context.PostAsync(botresponse);
        }
    }
}