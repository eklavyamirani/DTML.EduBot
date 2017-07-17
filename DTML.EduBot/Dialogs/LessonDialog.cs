using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdaptiveCards;
using DTML.EduBot.LessonPlan;
using Microsoft.Bot.Connector;

namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;

    [Serializable]
    public class LessonDialog : IDialog<string>
    {
        private Lesson lesson;
        private int currentTopic;

        public LessonDialog(Lesson lesson)
        {
            this.lesson = lesson;
            this.currentTopic = 0;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await PostAdaptiveCard(context);
            context.Wait(this.CheckAnswerAsync);
        }

        private async Task PostAdaptiveCard(IDialogContext context)
        {
            AdaptiveCard adaptiveCard = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = "This is ________"
                    },
                    new Image()
                    {
                        Size = ImageSize.Large,
                        Url  = "http://i1.wp.com/www.foodrepublic.com/wp-content/uploads/2012/03/roston_rotteneggs.jpg"
                    },
                    new TextInput()
                    {
                        Id = "Lesson",
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
        }

        private async Task CheckAnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value != null)
            {
                dynamic value = message.Value;

                if (value.Lesson.ToString().Equals(lesson.Topics.ToArray()[currentTopic].TopicName))
                {
                    await context.PostAsync("You have the right answer! Moving on to the next question.");
                    currentTopic++;
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
}