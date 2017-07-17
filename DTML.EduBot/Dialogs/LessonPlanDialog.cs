using System.Collections.Generic;
using System.Linq;
using DTML.EduBot.LessonPlan;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class LessonPlanDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string friendlyUserName = context.Activity.From.Name;

            ICollection<string> lessonsName = new List<string>();

            foreach (Lesson lesson in LessonPlanModule.LessonPlan().Lessons)
            {
                lessonsName.Add(lesson.LessonName);
            }

            PromptDialog.Choice(
                context,
                this.AfterLessonSelected,
                lessonsName,
                "Hi " + friendlyUserName + ",\n Which lesson would you like to go?",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: LessonPlanModule.LessonPlan().Lessons.Count);
        }

        private async Task AfterLessonSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                ICollection<Lesson> allLessons = LessonPlanModule.LessonPlan().Lessons;
                Lesson selectedLesson = allLessons.Where(lesson => selection.Equals(lesson.LessonName)).FirstOrDefault();
                context.Call(new LessonDialog(selectedLesson), AfterLessonFinished);
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterLessonFinished(IDialogContext context, IAwaitable<string> result)
        {
            
        }
    }
}