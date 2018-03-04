namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using DTML.EduBot.Constants;
    using DTML.EduBot.LessonPlan;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using UserData;
    using DTML.EduBot.Common;

    [Serializable]
    public class LessonPlanDialog : IDialog<string>
    {
        private const ulong _pointsPerLesson = 10;
        private ulong _userPointsBeforeLessonPlan;
        private static readonly IReadOnlyCollection<string> LevelChoices = new List<string> {
                Shared.LevelOne,
                Shared.LevelTwo};

        public LessonPlanDialog()
        {
        }

        public async Task StartAsync(IDialogContext context)
        {
            var _oldGamerProfile = GetUserGamerProfile(context);
            _userPointsBeforeLessonPlan = _oldGamerProfile.Points;
            var user = context.GetUserData();
            string friendlyUserName = user?.UserName;
            
            ICollection<string> lessonTitle = new List<string>();

            // get up to 5 lessons
            int i = 0;
            foreach (Lesson lesson in LessonPlanModule.LessonPlan.Lessons)
            {
                if (i >= 7) break;

                //var lessonInNativaLanguage = await MessageTranslator.TranslateTextAsync(lesson.LessonTitle, user?.NativeLanguageIsoCode);
                lessonTitle.Add(lesson.LessonTitle);
                i++;
            }

            var questionInNativeLanguage = await MessageTranslator.TranslateTextAsync($"Which lesson would you like to start?", user?.NativeLanguageIsoCode);

            PromptDialog.Choice(
                context,
                this.AfterLessonSelected,
                lessonTitle,
                $"{questionInNativeLanguage}",
                Shared.DoNotUnderstand,
                attempts: Shared.MaxPromptAttempts);

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
            var selection = await result;
            var selectedLesson = LessonPlanModule.LessonPlan.Lessons.FirstOrDefault(lesson => selection.Equals(lesson.LessonTitle));

            // TODO: inject dependency
            var badgeRepository = new Gamification.BadgeRepository();
            var updatedProfile = GetUserGamerProfile(context);
            var user = context.GetUserData();
            updatedProfile.Points += _pointsPerLesson;
            var newBadges = badgeRepository.GetEligibleBadges(updatedProfile, _userPointsBeforeLessonPlan);
            updatedProfile.Badges.AddRange(newBadges);

            var nm = new NotificationManager();
            nm.RecordEvent(EventType.GameCompleted.ToString(), selectedLesson?.LessonTitle, "LessonPlan", user.UserName);

            if (newBadges != null && newBadges.Any())
            {
                var tasks = newBadges.Select(async badge =>
                {
                    AdaptiveCard adaptiveCard = new AdaptiveCard()
                    {
                        Body = new List<CardElement>()
                        {
                            new Image()
                            {
                                Size = ImageSize.Medium,
                                Url  = "https://www.kastatic.org/images/badges/meteorite/thumbs-up-512x512.png"
                            },
                            new TextBlock()
                            {
                                Text = await MessageTranslator.TranslateTextAsync($"You unlocked {badge}", user?.NativeLanguageIsoCode),
                                Size = TextSize.Large,
                                Wrap = true
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
                });

                await Task.WhenAll(tasks);
            }

            // The current lesson finished. Plug in Analytics.
            var finalMessage = await result;
            await context.PostAsync(finalMessage);

            // Refactor
            var updatedUserData = context.GetUserData();
            updatedUserData.GamerProfile = updatedProfile;
            context.UpdateUserData(updatedUserData);
            context.Done(string.Empty);
        }

        private Gamification.GamerProfile GetUserGamerProfile(IDialogContext context)
        {
            var userData = context.GetUserData();
            return userData.GamerProfile;
        }
    }
}