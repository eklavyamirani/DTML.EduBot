namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
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

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string friendlyUserName = context.Activity.From.Name;
            
            PromptDialog.Choice(
                context,
                this.AfterChoiceSelected,
                Choices,
                "Hello Dear " + friendlyUserName + ",\n What would you like to do.",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: Choices.Count());

            return Task.CompletedTask;
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    // TODO: enum.
                    switch (selection)
                    {
                        case ChatWithBot:
                            await context.PostAsync("Great! Say Hello, and see what will I respond!");
                            context.Call(scope.Resolve<NoneDialog>(), this.AfterLessonPlan);
                            break;

                        case StartTheLessonPlan:
                            context.Call(scope.Resolve<LessonPlanDialog>(), this.AfterLessonPlan);
                            break;
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterLessonPlan(IDialogContext context, IAwaitable<object> result)
        {
            // BUG: this actually waits for user to respond. Needs to be proactive.
            await this.StartAsync(context);
        }
    }
}