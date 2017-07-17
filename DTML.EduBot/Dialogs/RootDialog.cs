using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<string>
    {
        private const string ChatWithBot = "Chat With Bot";
        private const string StartTheLessonPlan = "Start The Lesson Plan";

        private static readonly IEnumerable<string> Choices = new ReadOnlyCollection<string>
            (new List<String> {
                ChatWithBot,
                StartTheLessonPlan});

        private readonly LessonPlanDialog lessonPlanDialog;

        public RootDialog()
        {
            using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
            {
                lessonPlanDialog = WebApiApplication.FindContainer().Resolve<LessonPlanDialog>();
            }
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string friendlyUserName = context.Activity.From.Name;
            
            PromptDialog.Choice(
                context,
                this.AfterChoiceSelected,
                Choices,
                "Hello Dear " + friendlyUserName + ",\n What would you like to do.",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: Choices.Count());
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                switch (selection)
                {
                    case ChatWithBot:
                        await context.PostAsync("Great! Say Hello, and see what will I respond!");
                        context.Call(new RootQnaLuisDialog(), this.AfterLessonPlan);
                        break;

                    case StartTheLessonPlan:
                        await context.PostAsync("Great! let's get it started!");
                        context.Call(this.lessonPlanDialog, this.AfterLessonPlan);
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterLessonPlan(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var selection = (string) await result;

                switch (selection)
                {
                    case "success":
                        await context.PostAsync("Congratz! You made it! Type anything if you would like to try another question.");
                        await this.StartAsync(context);
                        break;

                    case "failed":
                        await context.PostAsync("It's ok, there is always next time :)");
                        await this.StartAsync(context);
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }
    }
}