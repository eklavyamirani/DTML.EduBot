namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using DTML.EduBot.Common;
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
            LaunchLessonPlan(dialogContext);
            return Task.CompletedTask;
        }

        private void AskToStartLessonPlan(IDialogContext context)
        {
            PromptDialog.Confirm(
                context, 
                TryStartLessonPlan, 
                BotPersonality.GetStartLessonPlanQuestion(), 
                BotPersonality.BotResponseToGibberish);
        }

        private async Task TryStartLessonPlan(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;
            if (confirm)
            {
                LaunchLessonPlan(context);
            }
            else
            {
                await context.PostAsync(BotPersonality.BotResponseToDeclinedLessonPlan);
                await context.PostAsync(BotPersonality.BuildAcquaintance());
            }
        }

        private void LaunchLessonPlan(IDialogContext context)
        {
            context.Call(_levelDialog, AfterLessonPlan);
        }

        private Task AfterLessonPlan(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
            return Task.CompletedTask;

        }
    }
}