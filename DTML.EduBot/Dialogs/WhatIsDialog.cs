using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace DTML.EduBot.Dialogs
{
    [LuisModel("31511772-4f1c-4590-87a8-0d6b8a7707a1", "a88bd2b022e34d5db56a73eb2bd33726")]
    [Serializable]
    public partial class WhatIsDialog : LuisDialog<object>
    {
        private const string BotName = "Zelda";

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't understand that");
        }

        [LuisIntent("WhatIs")]
        public async Task HandleBotName(IDialogContext context, LuisResult result)
        {
            if(result.Entities.Any(e => e.Type == "name"))
            {
                await context.PostAsync($"My name is {BotName}");
            }
            else
            {
                await context.PostAsync("Sorry, I didn't understand that");
            }
        }
    }
}