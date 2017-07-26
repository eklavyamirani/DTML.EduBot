namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.LessonPlan;
    using DTML.EduBot.Constants;
    using System.Collections.ObjectModel;

    [Serializable]
    public class LessonPlanDialog : IDialog<string>
    {
        private static readonly IEnumerable<string> LevelChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.LevelOne,
                Shared.LevelTwo,
                Shared.LevelThree,
                Shared.LevelFour,
                Shared.LevelFive});

        public Task StartAsync(IDialogContext context)
        {
            string friendlyUserName = context.Activity.From.Name;

            ICollection<string> lessonTitle = new List<string>();

            // get up to 5 lessons
            int i = 0;
            foreach (Lesson lesson in LessonPlanModule.LessonPlan.Lessons)
            {
                if (i >= 5) break;

                lessonTitle.Add(lesson.LessonTitle);
                i++;
            }

            PromptDialog.Choice(
                context,
                this.AfterLessonSelected,
                lessonTitle,
                $"{friendlyUserName} Which lesson would you like to start?",
                Shared.DoNotUnderstand,
                attempts: Shared.MaxAttempt);

            return Task.CompletedTask;
        }

        private async Task AfterLessonSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                var selectedLesson = LessonPlanModule.LessonPlan.Lessons.FirstOrDefault(lesson => selection.Equals(lesson.LessonTitle));
                if (selectedLesson == null)
                {
                    context.Fail(new InvalidOperationException("The selected lesson plan was null."));
                    return;
                }

                context.Call(new LessonDialog(selectedLesson), AfterLessonFinished);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterLessonFinished(IDialogContext context, IAwaitable<string> result)
        {
            // The current lesson finished. Plug in Analytics.
            var finalMessage = await result;
            await context.PostAsync(finalMessage);
            context.Done(string.Empty);
        }
    }
}