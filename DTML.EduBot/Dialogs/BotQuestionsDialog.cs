namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;
    using DTML.EduBot.Qna;
    using Microsoft.Azure;
    using Attributes;
    using System.Linq;
    using DTML.EduBot.Constants;

    [PreConfiguredLuisModel]
    [PreConfiguredQnaModel]
    [Serializable]
    public partial class BotQuestionsDialog : QnaLuisDialog<object>
    {
        [LuisIntent("BotQuestions")]
        public async Task HandleBotQuestionsIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Name))
            {
                await context.PostAsync("My name is " + BotPersonality.BotName);
            }
            else if (result.Entities.Any(e => e.Type == BotEntities.AboutMe))
            {
                await context.PostAsync("My name is " + BotPersonality.BotName
                    + " and I am a Bot on the web desgined to teach you English!");
            }
            else
            {
                await context.PostAsync("I'm not sure what you wanted me to say");
            }
        }

    }
}