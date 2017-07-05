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
    public partial class RootDialog : LuisDialog<object>
    {
        [LuisIntent("UserInformation")]
        public async Task HandleUserInformation(IDialogContext context, LuisResult result)
        {
            EntityRecommendation entity;
            string BotResponse = BotPersonality.BotResponseToUserName;
            if (result.TryFindEntity("name", out entity))
            {
                ChatContext.Username = entity.Entity;
                BotResponse += " " + ChatContext.Username;
            }

            BotResponse += "! <br/>" + BotPersonality.GetChatContinuer();
            await context.PostAsync(BotResponse);
        }
    }
}