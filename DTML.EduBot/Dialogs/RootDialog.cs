﻿namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;
    using DTML.EduBot.Qna;

    [LuisModel("", "")]
    [QnaModel(hostUri: "", subscriptionKey: "", modelId: "")]
    [Serializable]
    public partial class RootDialog : QnaLuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            await ConvertAndPostResponse(context, BotPersonality.BotResponseUnrecognizedIntent);
        }

       [LuisIntent("Gibberish")]
        public async Task HandleGibberish(IDialogContext context, LuisResult result)
        {
            await ConvertAndPostResponse(context, BotPersonality.BotResponseToGibberish);
        }

       
    }
}