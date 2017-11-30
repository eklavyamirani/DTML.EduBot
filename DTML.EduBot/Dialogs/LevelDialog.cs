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
        private static readonly IReadOnlyCollection<string> LevelChoices = new List<String> {
                Shared.LevelOne,
                Shared.LevelTwo,
                Shared.LevelThree};

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
                "How much English do you already know?",
                Shared.DoNotUnderstand,
                attempts: Shared.MaxPromptAttempts);

            return Task.CompletedTask;
        }

        private async Task AfterLevelSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;
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