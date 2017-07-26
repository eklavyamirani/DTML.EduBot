namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        private LevelDialog _levelDialog;

        public ChitChatDialog(LevelDialog levelDialog)
        {
            _levelDialog = levelDialog;
        }

        [LuisIntent("LearnEnglish")]
        public Task HandleLessonPlan(IDialogContext dialogContext, LuisResult luisResult)
        {
            dialogContext.Call(_levelDialog, AfterLessonPlan);
            return Task.CompletedTask;
        }

        private Task AfterLessonPlan(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
            return Task.CompletedTask;

        }
    }
}