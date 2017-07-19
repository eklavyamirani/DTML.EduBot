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
    public partial class RootDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Age")]
        public async Task HandleAgeIntent(IDialogContext context, LuisResult result)
        {
            string botresponse = $"I am quite young. Just couple month old. But I am already a Professor. How about that !";
            await ConvertAndPostResponse(context, botresponse);
        }

        [LuisIntent("WhatIs")]
        public async Task HandleWhatIsIntent(IDialogContext context, LuisResult result)
        {
            string botresponse;
            if (result.Entities.Any(e => e.Type == BotEntities.Name))
            {
                botresponse = $"My name is {BotPersonality.BotName}";
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.Time))
            {
                botresponse=$"It's always morning in the Botland, so I never need to sleep";
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.Date))
            {
                var date = DateTime.Now.ToLongDateString();
                botresponse = $"Oh, that's easy... It is {date}";
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.encyclopediaOrganization))
            {
                //TODO: Call Wikipedia API and provide answer
                botresponse = $"I think it is a name of some company...";
            }
            else
            {
                botresponse = BotPersonality.GetRandomGenericResponse();
            }

            await ConvertAndPostResponse(context, botresponse);
        }
    }
}