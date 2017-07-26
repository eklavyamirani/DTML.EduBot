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
using DTML.EduBot.Constants;

namespace DTML.EduBot.Dialogs
{
    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        [LuisIntent("UserInformation")]
        public async Task HandleUserInformation(IDialogContext context, LuisResult result)
        {
            EntityRecommendation entity;
            string BotResponse = BotPersonality.GetRandomGenericResponse() + Shared.ClientNewLine;

            if (result.TryFindEntity(BotEntities.Name, out entity))
            {

                ChatContext.Username = entity.Entity;
                BotResponse = BotPersonality.BotResponseToUserName + " " + ChatContext.Username + "! " + Shared.ClientNewLine;
            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Hobby))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Animal))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Place))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Language))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Family))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Color))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Subject))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Food))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Drink))
            {

            }


            BotResponse += BotPersonality.BuildAcquaintance();
            await context.PostAsync(BotResponse);
        }
    }
}