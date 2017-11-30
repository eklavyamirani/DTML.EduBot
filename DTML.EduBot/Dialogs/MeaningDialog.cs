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
    public partial class MeaningDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Meaning")]
        public async Task HandleMeaningIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.TranslateText))
            {
                //TODO: implement dictionary api to get the definition, pronunciation, part of speech, synonnyms, antonyms, etc
                await context.PostAsync("This is the definition of a word");
            }
            else
            {
                await context.PostAsync("What is the word you would like to define?");
            }
            
        }
    }
}