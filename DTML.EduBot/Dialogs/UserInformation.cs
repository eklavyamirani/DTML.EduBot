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
    public partial class RootDialog : LuisDialog<object>
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

            BotResponse += BotPersonality.BuildAcquaintance();
            await context.PostAsync(BotResponse);
        }
    }
}