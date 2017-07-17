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
    public partial class RootQnaLuisDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Age")]
        public async Task HandleAgeIntent(IDialogContext context, LuisResult result)
        {
                await context.PostAsync($"I am quite young. Just couple month old. But I am already a Professor. How about that !");
        }

        [LuisIntent("WhatIs")]
        public async Task HandleWhatIsIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Name))
            {
                await context.PostAsync($"My name is {BotPersonality.BotName}");
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.Time))
            {
                await context.PostAsync($"It's always morning in the Botland, so I never need to sleep");
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.Date))
            {
                var date = DateTime.Now.ToLongDateString();
                await context.PostAsync($"Oh, that's easy... It is {date}");
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.encyclopediaOrganization))
            {
                //TODO: Call Wikipedia API and provide answer
                await context.PostAsync($"I think it is a name of some company...");
            }
            else
            {
                await context.PostAsync(BotPersonality.GetRandomGenericResponse());
            }
        }
    }
}