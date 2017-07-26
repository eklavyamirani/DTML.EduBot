namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using DTML.EduBot.Constants;
    using System.Collections.ObjectModel;
    using Autofac;

    [Serializable]
    public class LevelDialog : IDialog<string>
    {
        private static readonly IEnumerable<string> LevelChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.LevelOne,
                Shared.LevelTwo,
                Shared.LevelThree,
                Shared.LevelFour,
                Shared.LevelFive});

        private readonly LessonPlanDialog _lessonPlanDialog;

        public LevelDialog(LessonPlanDialog lessonPlanDialog)
        {
            _lessonPlanDialog = lessonPlanDialog;
        }

        public Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(
                context,
                this.AfterLevelSelected,
                LevelChoices,
                "Which level are you?",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: Shared.MaxAttempt);

            return Task.CompletedTask;
        }

        private async Task AfterLevelSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                // TODO: choose lesson plan based on level selection. currently just automatically navigating to
                // the one lesson plan that we have
                context.Call(_lessonPlanDialog, this.AfterLevelFinished);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private Task AfterLevelFinished(IDialogContext context, IAwaitable<string> result)
        {
            context.Done(string.Empty);
            return Task.CompletedTask;
        }

    }
}