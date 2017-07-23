namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.LessonPlan;

    [Serializable]
    public class LessonPlanDialog : IDialog<string>
    {
        private const int maxRetries = 3;

        public Task StartAsync(IDialogContext context)
        {
            string friendlyUserName = context.Activity.From.Name;

            ICollection<string> lessonTitle = new List<string>();

            foreach (Lesson lesson in LessonPlanModule.LessonPlan.Lessons)
            {
                lessonTitle.Add(lesson.LessonTitle);
            }

            PromptDialog.Choice(
                context,
                this.AfterLessonSelected,
                lessonTitle,
                $"{friendlyUserName} Which lesson would you like to start?",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: maxRetries);

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
<<<<<<< HEAD

            await this.StartAsync(context);
=======
            context.Done(string.Empty);
>>>>>>> 7941c63b0b4cc99b6b51a1510ab84dfbbf877389
        }
    }
}