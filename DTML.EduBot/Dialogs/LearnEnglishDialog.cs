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

    [PreConfiguredLuisModel]
    [PreConfiguredQnaModel]
    [Serializable]
    public partial class LearnEnglishDialog : QnaLuisDialog<object>
    {
        [LuisIntent("LearnEnglish")]
        public async Task HandleLearnEnglishIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("");
        }
    }
}