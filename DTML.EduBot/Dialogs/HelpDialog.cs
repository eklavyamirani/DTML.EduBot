using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DTML.EduBot.Dialogs
{
    public partial class HelpDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Help")]
        public async Task HandleHelpIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("");
        }

        [LuisIntent("Repeat")]
        public async Task HandleRepeatIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("");
        }
    }
}