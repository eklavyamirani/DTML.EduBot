namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using DTML.EduBot.Common;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        [LuisIntent("LearnEnglish")]
        public Task HandleLessonPlan(IDialogContext dialogContext, LuisResult luisResult)
        {
            LaunchLessonPlan(dialogContext);
            return Task.CompletedTask;
        }

        private async Task AskToStartLessonPlan(IDialogContext context)
        {
            string translatedBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.GetStartLessonPlanQuestion());

            PromptDialog.Confirm(
                context, 
                TryStartLessonPlan,
                translatedBotResponse,
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
                string translatedDeclineBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.BotResponseToDeclinedLessonPlan);
                await context.PostAsync(translatedDeclineBotResponse);

                string translatedAcquaitanceBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.BuildAcquaintance());
                await context.PostAsync(translatedAcquaitanceBotResponse);
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