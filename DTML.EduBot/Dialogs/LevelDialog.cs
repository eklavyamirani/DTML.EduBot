namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using DTML.EduBot.Common;
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

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(BotPersonality.BotEnteringEnglish);

            PromptDialog.Choice(
                context,
                this.AfterLevelSelected,
                LevelChoices,
                BotPersonality.BotAskingLevel,
                Shared.DoNotUnderstand,
                attempts: Shared.MaxAttempt);
        }

        private async Task AfterLevelSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                // TODO: choose lesson plan based on level selection. currently just automatically navigating to
                // the one lesson plan that we have
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    context.Call(scope.Resolve<LessonPlanDialog>(), this.AfterLevelFinished);
                }
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