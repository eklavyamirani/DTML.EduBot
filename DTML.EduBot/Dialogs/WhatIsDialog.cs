using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
namespace DTML.EduBot.Dialogs
{
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Constants;
    using DTML.EduBot.Common;
    using DTML.EduBot.Extensions;
    using System;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {

        [LuisIntent("WhatIs")]
        public async Task HandleWhatIsIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Name, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"My name is {BotPersonality.BotName}");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.encyclopediaOrganization, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"I think it is a company...");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Color, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"My favorite color is blue. Blue is the color of light between violet and green on the visible spectrum... It is chosen by almost half of both men and women as their favourite colour.");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Time, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"Right now is {DateTime.Now.ToShortTimeString()} at Seattle, WA");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Date, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                var date = DateTime.Now.ToLongDateString();
                await context.PostTranslatedAsync($"Oh yeah, let me check my digital calendar. Today is... {date}");
            }
            else
            {
                await context.PostTranslatedAsync(BotPersonality.GetRandomGenericResponse());
            }
        }
    }
}