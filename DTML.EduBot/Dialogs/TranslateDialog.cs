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
        [LuisIntent("Translate")]
        public async Task HandleTranslateIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("");
        }
    }
}