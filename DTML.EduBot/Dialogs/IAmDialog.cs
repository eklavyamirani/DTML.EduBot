﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DTML.EduBot.Common;
using DTML.EduBot.Constants;

namespace DTML.EduBot.Dialogs
{
    public partial class ChitChatDialog : QnaLuisDialog <Object>
    {
        [LuisIntent("UserEmotion")]
        public async Task HandleIAmIntent(IDialogContext context, LuisResult result)
        {
            string translatedBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.GetRandomGenericResponse());

            await context.PostAsync(translatedBotResponse);
        }
    }
}