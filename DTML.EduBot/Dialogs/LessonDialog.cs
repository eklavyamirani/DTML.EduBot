namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using DTML.EduBot.LessonPlan;
    using DTML.EduBot.Constants;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;
    using System.Collections.ObjectModel;
    using Microsoft.Bot.Builder.Luis;
    using DTML.EduBot.Extensions;
    using DTML.EduBot.Common;
    using DTML.EduBot.UserData;

    [Serializable]
    public class LessonDialog : LuisDialog<string>
    {
        private readonly int MAX_ATTEMPT = 2;
        private Lesson lesson;

        private static readonly IEnumerable<string> YesNoChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.Yes,
                Shared.No });

        public LessonDialog(Lesson lesson)
        {
            this.lesson = lesson;
        }

        public override async Task StartAsync(IDialogContext context)
        {
            var wasSuccess = await PostAdaptiveCard(context);
            if (!wasSuccess)
            {
                context.Done(Shared.NoMoreLessonsMessage);
                return;
            }

            context.Wait(this.CheckAnswerOptionsAsync);
        }

        private async Task<bool> PostAdaptiveCard(IDialogContext context)
        {
            var nextTopic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (nextTopic == null)
            {
                return false;
            }

            List<ActionBase> answerOptions = PopulateActionBasesFromAnswerOptions(nextTopic.AnswerOptions);

            AdaptiveCard adaptiveCard = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = nextTopic.Question,
                        Wrap = true
                    },
                    new Image()
                    {
                        Size = ImageSize.Large,
                        Url  = nextTopic.ImageUrl,
                        HorizontalAlignment= HorizontalAlignment.Center
                    },
                },
                Actions = answerOptions
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

        private List<ActionBase> PopulateActionBasesFromAnswerOptions(ICollection<string> answerOptions)
        {
            List<ActionBase> actionBases = new List<ActionBase>();

            foreach (string answer in answerOptions)
            {
                actionBases.Add(
                    new SubmitAction()
                    {
                        Title = answer,
                        DataJson = "{\"answer\": \"" + answer + "\"}"
                    }
                );
            }

            return actionBases;
        }

        private async Task CheckAnswerOptionsAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            dynamic response = ExtractMessageValue(result);
            StudentResponse studentResponse = StudentResponse.FromDynamic(response);

            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            if (studentResponse.Answer != null && topic.CorrectAnswers.Any(a => a.Equals(studentResponse.Answer, StringComparison.InvariantCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync(topic.CorrectAnswerBotResponse);
                context.Wait(this.CheckTypedAnswerAsync);
            }
            else
            {
                await Task.WhenAll(context.PostTranslatedAsync(topic.WrongAnswerBotResponse),
                    this.StartAsync(context));
            }
        }

        private async Task CheckTypedAnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            StudentResponse studentResponse = new StudentResponse(message.Text);

            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            if (studentResponse.Answer != null && topic.CorrectAnswers.Any(a => a.Equals(studentResponse.Answer, StringComparison.InvariantCultureIgnoreCase)))
            {
                await this.PronounceLearnedPhrase(context, result);
            }
            else
            {
                await Task.WhenAll(context.PostTranslatedAsync(topic.WrongAnswerBotResponse),
                    this.StartAsync(context));
            }
        }

        private async Task PronounceLearnedPhrase(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            await context.PostAsync(Shared.CorrectAnswerMessage);
            await this.PostAudioInstruction(context, topic);
            context.Wait(CheckAudioExercise);
        }

        private async Task PostAudioInstruction(IDialogContext context, Topic topic)
        {
            await context.PostAsync(Shared.RepeatAfterMe);
            await context.SayAsync(topic.CorrectAnswers.First(), topic.CorrectAnswers.First(), new MessageOptions
            {
                InputHint = "expectingInput"
            });
        }

        private async Task CheckAudioExercise(IDialogContext context, IAwaitable<IMessageActivity> responseActivity)
        {
            var response = await responseActivity;
            // TODO: property
            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            if (response == null || !topic.CorrectAnswers.Any(a=> a.Equals(response.Text, StringComparison.InvariantCultureIgnoreCase)))
            {
                await this.PostAudioInstruction(context, topic);
                return;
            }

            await Task.WhenAll(context.PostAsync(Shared.CorrectAnswerMessage),
                               this.WrapUpCurrentTopic(context));
        }

        private async Task WrapUpCurrentTopic(IDialogContext context)
        {
            string topicMessage = Shared.TopicCompleteMessage;

            // display number of stars for number of topics done
            for (int i = 0; i < lesson.currentTopic + 1; i++)
            {
                topicMessage += "\U00002B50";
            }

            await context.PostTranslatedAsync(topicMessage);

            if (lesson.currentTopic >= lesson.Topics.Count - 1)
            // after finish last topic, ask if they want to take lesson's quiz
            {
                await context.PostAsync(Shared.AllTopicsCompleteMessage);
                PromptDialog.Choice(
                    context,
                    this.AfterWrapUpCurrentLesson,
                    YesNoChoices,
                    Shared.ReadyForQuiz,
                    Shared.DoNotUnderstand,
                    attempts: MAX_ATTEMPT);
            }
            else
            {
                lesson.currentTopic++;
                await this.StartAsync(context);
            }
        }

        private async Task AfterWrapUpCurrentLesson(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var user = context.GetUserData();
                var nm = new NotificationManager();
                nm.RecordEvent(EventType.GameCompleted.ToString(), lesson.LessonTitle, "LessonPlan", user.UserName);

                var selection = await result as string;
                if (selection.Equals(Shared.Yes, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new QuizDialog(lesson.Quiz), this.AfterQuizFinished);
                }
                else
                {
                    context.Done(Shared.LessonCompleteMessage);
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostTranslatedAsync("It looks like you are not ready.");
                context.Done(Shared.LessonCompleteMessage);
            }
        }

        private async Task AfterQuizFinished(IDialogContext context, IAwaitable<string> result)
        {
            // The current lesson finished. Plug in Analytics.
            var finalMessage = await result;
            await context.PostAsync(finalMessage);
            context.Done(Shared.LessonCompleteMessage);
        }

        private async Task<dynamic> ExtractMessageValue(IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value == null)
            {
                if (message.Text != null)
                {
                    return message.Text;
                }
                return null;
            }

            dynamic response = message.Value;

            return response;
        }
    }
}