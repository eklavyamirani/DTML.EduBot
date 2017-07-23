namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using DTML.EduBot.LessonPlan;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;

    [Serializable]
    public class LessonDialog : IDialog<string>
    {
        private Lesson lesson;

        public LessonDialog(Lesson lesson)
        {
            this.lesson = lesson;
        }

        public async Task StartAsync(IDialogContext context)
        {
            var wasSuccess = await PostAdaptiveCard(context);
            if (!wasSuccess)
            {
                context.Done(Constants.Shared.NoMoreLessonsMessage);
                return;
            }

            context.Wait(this.CheckAnswerAsync);
        }

        private async Task<bool> PostAdaptiveCard(IDialogContext context)
        {
            var nextTopic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (nextTopic == null)
            {
                return false;
            }

            // TODO: Remove hard coded stuff.
            AdaptiveCard adaptiveCard = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = nextTopic.Question
                    },
                    new Image()
                    {
                        Size = ImageSize.Large,
                        Url  = "http://i1.wp.com/www.foodrepublic.com/wp-content/uploads/2012/03/roston_rotteneggs.jpg"
                    },
                    new TextInput()
                    {
                        Id = "Answer",
                        Style = TextInputStyle.Text
                    }
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Submit Answer",
                        DataJson = "{ \"Type\": \"Topic\"}"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);
            return true;
        }

        private async Task CheckAnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value == null)
            {
                return;
            }

            dynamic response = message.Value;
            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            StudentResponse studentResponse = StudentResponse.FromDynamic(response);
            // TODO: check for typos in answer.
            if (studentResponse.Answer.Equals(topic.CorrectAnswer, StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync("You have the right answer! Moving on to the next question.");
                lesson.currentTopic++;
                await this.StartAsync(context);
            }
            else
            {
                await context.PostAsync("Please try again");
                await this.StartAsync(context);
            }
        }
    }
}