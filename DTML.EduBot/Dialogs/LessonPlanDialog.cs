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
    using UserData;
    using Autofac;
    using AdaptiveCards;
    using System.Threading;

    [Serializable]
    public class LessonPlanDialog : IDialog<string>
    {
        private const ulong _pointsPerLesson = 10;
        private ulong _userPointsBeforeLessonPlan;
        private IUserDataRepository _userDataRepository;
        private static readonly IEnumerable<string> LevelChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.LevelOne,
                Shared.LevelTwo,
                Shared.LevelThree,
                Shared.LevelFour,
                Shared.LevelFive});

        public LessonPlanDialog(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        public Task StartAsync(IDialogContext context)
        {
            var _oldGamerProfile = GetUserGamerProfile(context.Activity.From.Id);
            _userPointsBeforeLessonPlan = _oldGamerProfile.Points;
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
            // TODO: inject dependency
            var badgeRepository = new Gamification.BadgeRepository();
            var updatedProfile = GetUserGamerProfile(context.Activity.From.Id);
            updatedProfile.Points += _pointsPerLesson;
            var newBadges = badgeRepository.GetEligibleBadges(updatedProfile, _userPointsBeforeLessonPlan);
            updatedProfile.Badges.AddRange(newBadges);

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
                                Text = $"You unlocked {badge}",
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
            var updatedUserData = _userDataRepository.GetUserData(context.Activity.From.Id);
            updatedUserData.GamerProfile = updatedProfile;
            _userDataRepository.UpdateUserData(updatedUserData);
            context.Done(string.Empty);
        }

        private Gamification.GamerProfile GetUserGamerProfile(string userId)
        {
            var userData = _userDataRepository.GetUserData(userId);
            if (userData == null)
            {
                userData = new UserData();
            }

            return userData.GamerProfile;
        }
    }
}