namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.Common;
    using DTML.EduBot.Constants;
    using DTML.EduBot.UserData;

    [Serializable]
    public class RootDialog : IDialog<string>
    {
        private readonly ChitChatDialog _chitChatDialog;

        private static readonly IEnumerable<string> YesNoChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.Yes,
                Shared.No});
        
        private static readonly IEnumerable<string> DialogChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.ChatWithBot,
                Shared.StartTheLessonPlan});

        public RootDialog(ChitChatDialog chitChatDialog)
        {
            _chitChatDialog = chitChatDialog;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userText = context.Activity.From.Name;

            try
            {
                userText = (await result).Text;
            }
            catch (Exception)
            {
                // Swallow exception for the demo purpose
                // TODO log the exception
            }
            
            string detectedLanguageIsoCode = await MessageTranslator.IdentifyLangAsync(userText);

            using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
            {
                var userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);
                if (userData == null)
                {
                    userData = new UserData();
                    userData.UserId = context.Activity.From.Id;
                    userData.NativeLanguageIsoCode = detectedLanguageIsoCode;
                }

                scope.Resolve<IUserDataRepository>().UpdateUserData(userData);

                if (MessageTranslator.DEFAULT_LANGUAGE.Equals(detectedLanguageIsoCode))
                {
                    await context.PostAsync(BotPersonality.UserNameQuestion);

                    // detected it's english language
                    context.Wait(this.UserNameReceivedAsync);
                }
                else
                {
                    // detected user language

                    CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                    CultureInfo detectedCulture =
                       allCultures
                            .Where(cultureInfo => (detectedLanguageIsoCode.Contains(cultureInfo.TwoLetterISOLanguageName)))
                            .FirstOrDefault();

                    string detectedLanguageName = CultureInfo.GetCultureInfo(MessageTranslator.DEFAULT_LANGUAGE).DisplayName;

                    if (detectedCulture != null)
                    {
                        detectedLanguageName = detectedCulture.DisplayName;
                    }

                    string translatedSwitchQuestion = await MessageTranslator.TranslateTextAsync($"Do you want to switch to {detectedLanguageName}", detectedLanguageIsoCode);

                    string translatedDontUnderstand = await MessageTranslator.TranslateTextAsync(Shared.DoNotUnderstand, detectedLanguageIsoCode);

                    PromptDialog.Choice(
                        context,
                        this.AfterChoosingLanguageSwitch,
                        await MessageTranslator.TranslatedChoices(YesNoChoices, detectedLanguageIsoCode),
                        translatedSwitchQuestion,
                        translatedDontUnderstand,
                        attempts: Shared.MaxAttempt
                    );
                }
            }
        }

        private async Task UserNameReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var messageActivity = await result;
            await context.Forward(_chitChatDialog, this.AfterChitChatComplete, messageActivity, CancellationToken.None);
        }

        private async Task UserNameReceivedInNativeLanguageAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity userText = await result;
            string userTextInEnglish = await MessageTranslator.TranslateTextAsync(userText.Text);
            userText.Text = userTextInEnglish;
            await context.Forward(_chitChatDialog, this.AfterChitChatComplete, userText, CancellationToken.None);
        }

        private async Task AfterChitChatComplete(IDialogContext context, IAwaitable<object> result)
        {
            // Chit chat should never end, error if we get here
            await context.PostAsync("Sorry, I seem to be getting tired here, I'll take a power nap and get back to you!");
            await this.StartAsync(context);
        }

        private async Task AfterDialogEnded(IDialogContext context, IAwaitable<object> result)
        {
            // BUG: this actually waits for user to respond. Needs to be proactive.		 +            try
            await this.StartAsync(context);
        }

        private async Task AfterChoosingLanguageSwitch(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result as string;

            try
            {
                using(var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    UserData userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);

                    string translatedYes = await MessageTranslator.TranslateTextAsync(Shared.Yes.ToString(), userData.NativeLanguageIsoCode);
                    string translatedNo = await MessageTranslator.TranslateTextAsync(Shared.No, userData.NativeLanguageIsoCode);

                    if (translatedYes.Equals(response, StringComparison.OrdinalIgnoreCase))
                    {
                        string translatedSelfIntroduction =
                            await MessageTranslator.TranslateTextAsync(BotPersonality.BotSelfIntroduction,
                                userData.NativeLanguageIsoCode);
                        await context.PostAsync($"{translatedSelfIntroduction}");

                        string translatedUserNameQuestion =
                            await MessageTranslator.TranslateTextAsync(BotPersonality.UserNameQuestion,
                                userData.NativeLanguageIsoCode);
                        await context.PostAsync($"{translatedUserNameQuestion}");

                        context.Wait(this.UserNameReceivedInNativeLanguageAsync);
                    }
                    else if (translatedNo.Equals(response, StringComparison.OrdinalIgnoreCase))
                    {
                        await context.PostAsync($"{BotPersonality.UserNameQuestion}");

                        context.Wait(this.UserNameReceivedAsync);
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
                using(var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    UserData userData =
                        scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);

                    string translatedTooManyAttemptMessage = await MessageTranslator.TranslateTextAsync(Shared.TooManyAttemptMessage, userData.NativeLanguageIsoCode);

                    await context.PostAsync($"{translatedTooManyAttemptMessage}");
                }

                await this.StartAsync(context);
            }
        }
    }
}