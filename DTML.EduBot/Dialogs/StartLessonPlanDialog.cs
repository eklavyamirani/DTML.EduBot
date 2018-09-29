namespace DTML.EduBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using Common;
    using Extensions;
    using DTML.EduBot.UserData;
    using DTML.EduBot.Common.Interfaces;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        private LevelDialog _levelDialog;

        public ChitChatDialog(LevelDialog levelDialog, ILogger logger) : base(logger)
        {
            _levelDialog = levelDialog;
        }

        [LuisIntent("LearnEnglish")]
        public Task HandleLessonPlan(IDialogContext dialogContext, LuisResult luisResult)
        {
            LaunchLessonPlan(dialogContext);
            return Task.CompletedTask;
        }

        private async Task AskToStartLessonPlan(IDialogContext context)
        {
            var possibleResponses = new List<string>()
            {
                BotPersonality.GetStartLessonPlanQuestion(),
                BotPersonality.BotResponseToGibberish
            };

            IReadOnlyCollection<string> translatedResponses;
            string language;
            if (context.UserData.TryGetValue(Constants.Shared.UserLanguageCodeKey, out language) && language != MessageTranslator.DEFAULT_LANGUAGE)
            {
                translatedResponses = await MessageTranslator.TranslateTextAsync(possibleResponses, language);
            }
            else
            {
                translatedResponses = possibleResponses;
            }

            PromptDialog.Confirm(
                context, 
                TryStartLessonPlan, 
                translatedResponses.ElementAtOrDefault(0) ?? string.Empty,
                translatedResponses.ElementAtOrDefault(1) ?? string.Empty);
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
                await context.PostTranslatedAsync(BotPersonality.BotResponseToDeclinedLessonPlan);
                await context.PostTranslatedAsync(BotPersonality.BuildAcquaintance());
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