namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        private LessonPlanDialog _lessonPlanDialog;

        public ChitChatDialog(LessonPlanDialog lessonPlanDialog)
        {
            _lessonPlanDialog = lessonPlanDialog;
        }

        [LuisIntent("LearnEnglish")]
        public Task HandleLessonPlan(IDialogContext dialogContext, LuisResult luisResult)
        {
            dialogContext.Call(_lessonPlanDialog, AfterLessonPlan);
            return Task.CompletedTask;
        }

        private Task AfterLessonPlan(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
            return Task.CompletedTask;

        }
    }
}