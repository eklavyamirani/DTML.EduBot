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
    using System.Collections.ObjectModel;

    [Serializable]
    public class LessonDialog : IDialog<string>
    {
        private readonly int MAX_ATTEMPT = 2;
        private Lesson lesson;

        private static readonly IEnumerable<string> YesNoChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Constants.Shared.Yes,
                Constants.Shared.No });

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
                        Text = nextTopic.Question
                    },
                    new Image()
                    {
                        Size = ImageSize.Large,
                        Url  = nextTopic.ImageUrl
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

            if (studentResponse.Answer != null && studentResponse.Answer.Equals(topic.CorrectAnswer, StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync(topic.CorrectAnswerBotResponse);
                context.Wait(this.CheckTypedAnswerAsync);
            }
            else
            {
                await context.PostAsync(topic.WrongAnswerBotResponse);
                await this.StartAsync(context);
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

            if (studentResponse.Answer != null && studentResponse.Answer.Equals(topic.CorrectAnswer, StringComparison.InvariantCultureIgnoreCase))
            {
                await this.PronounceLearnedPhrase(context, result);
            }
            else
            {
                await context.PostAsync(topic.WrongAnswerBotResponse);
                await this.StartAsync(context);
            }
        }

        private async Task PronounceLearnedPhrase(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            await context.PostAsync(Constants.Shared.CorrectAnswerMessage);
            await this.PostAudioInstruction(context, topic);
            context.Wait(CheckAudioExercise);
        }

        private async Task PostAudioInstruction(IDialogContext context, Topic topic)
        {
            await context.PostAsync(Constants.Shared.RepeatAfterMe);

            // client handling at: https://github.com/eklavyamirani/BotFramework-WebChat/commit/a0cc2cf87563414c558691583788bbd8e8c8f6a2
            await context.SayAsync(topic.CorrectAnswer, topic.CorrectAnswer, new MessageOptions
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

            if (response == null || !topic.CorrectAnswer.Equals(response.Text, StringComparison.InvariantCultureIgnoreCase))
            {
                await this.PostAudioInstruction(context, topic);
                return;
            }

            await context.PostAsync(Constants.Shared.CorrectAnswerMessage);
            await this.WrapUpCurrentTopic(context, topic);
        }

        private Task WrapUpCurrentTopic(IDialogContext context, Topic topic)
        {
            ICollection<string> wrapUpPhrases = new List<string>();
            wrapUpPhrases.Add(topic.NextTopicPhrase);
            wrapUpPhrases.Add(topic.StayOnCurrentTopicPhrase);

            PromptDialog.Choice(
                context,
                this.AfterWrapUpCurrentTopic,
                wrapUpPhrases,
                "This marks the end of current topic!",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: MAX_ATTEMPT);

            return Task.CompletedTask;
        }

        private async Task AfterWrapUpCurrentTopic(IDialogContext context, IAwaitable<string> result)
        {
            var topic = lesson.Topics.ElementAtOrDefault(lesson.currentTopic);
            if (topic == null)
            {
                return;
            }

            try
            {
                var selection = await result as string;
                
                if (topic.NextTopicPhrase.Equals(selection))
                {
                    if (lesson.currentTopic >= lesson.Topics.Count - 1)
                        // after finish last topic, ask if they want to take lesson's quiz
                    {
                        await context.PostAsync("You completed all the topics. Congratulations!");
                        PromptDialog.Choice(
                            context,
                            this.AfterWrapUpCurrentLesson,
                            YesNoChoices,
                            "Are you ready for the quiz?",
                            "I am sorry but I didn't understand that. I need you to select one of the options below",
                            attempts: MAX_ATTEMPT);
                    }
                    else
                    {
                        lesson.currentTopic++;
                        await this.StartAsync(context);
                    }
                }
                else if (topic.StayOnCurrentTopicPhrase.Equals(selection))
                {
                    // TODO: Why null?
                    await this.PronounceLearnedPhrase(context, null);
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterWrapUpCurrentLesson(IDialogContext context, IAwaitable<string> result)
        {
            var selection = await result as string;
            if (selection.Equals(Constants.Shared.Yes, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new QuizDialog(lesson.Quiz), this.AfterQuizFinished);
            }
            else
            {
                context.Done("This is the end of the current lesson. Thank you!");
            }
        }

        private async Task AfterQuizFinished(IDialogContext context, IAwaitable<string> result)
        {
            // The current lesson finished. Plug in Analytics.
            var finalMessage = await result;
            await context.PostAsync(finalMessage);
            context.Done("This is the end of the current lesson. Thank you!");
        }

        private async Task<dynamic> ExtractMessageValue(IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value == null)
            {
                return null;
            }

            dynamic response = message.Value;

            return response;
        }
    }
}