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
    public partial class NavigateDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Navigate")]
        public async Task HandleNavigateIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("");
        }
    }
}