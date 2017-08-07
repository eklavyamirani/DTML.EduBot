namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.Common;
    using DTML.EduBot.Constants;
    using DTML.EduBot.UserData;

    [Serializable]
    public class RootDialog : IDialog<string>
    {
        private readonly ChitChatDialog _chitChatDialog;
        private readonly IUserDataRepository _userDataRepository;

        private static readonly IReadOnlyCollection<string> YesNoChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.Yes,
                Shared.No});

        private static readonly IReadOnlyCollection<string> DialogChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.ChatWithBot,
                Shared.StartTheLessonPlan});

        public RootDialog(ChitChatDialog chitChatDialog, IUserDataRepository userDataRepository)
        {
            this._chitChatDialog = chitChatDialog;
            this._userDataRepository = userDataRepository;
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

            var userData = _userDataRepository.GetUserData(context.Activity.From.Id);
            if (userData == null)
            {
                userData = new UserData();
                userData.UserId = context.Activity.From.Id;
                userData.NativeLanguageIsoCode = detectedLanguageIsoCode;
            }

            _userDataRepository.UpdateUserData(userData);

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
                        .FirstOrDefault(cultureInfo => (detectedLanguageIsoCode.Contains(cultureInfo.TwoLetterISOLanguageName)));

                string detectedLanguageName = CultureInfo.GetCultureInfo(MessageTranslator.DEFAULT_LANGUAGE).DisplayName;

                if (detectedCulture != null)
                {
                    detectedLanguageName = detectedCulture.DisplayName;
                }

                var translationInputTextlist = new List<string>(capacity: 4);
                translationInputTextlist.AddRange(YesNoChoices);
                translationInputTextlist.AddRange(new string[] { $"Do you want to switch to {detectedLanguageName}", Shared.DoNotUnderstand });

                var translatedList = await MessageTranslator.TranslateTextAsync(translationInputTextlist, detectedLanguageIsoCode);
                var translatedSwitchQuestion = translatedList.ElementAt(2);
                var translatedDontUnderstand = translatedList.ElementAt(3);
                var translatedChoices = translatedList.Take(2);

                PromptDialog.Choice(
                    context,
                    this.AfterChoosingLanguageSwitch,
                    translatedChoices,
                    translatedSwitchQuestion,
                    translatedDontUnderstand,
                    attempts: Shared.MaxPromptAttempts
                );
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
            await Task.WhenAll(context.PostAsync("Sorry, I seem to be getting tired here, I'll take a power nap and get back to you!"),
                        this.StartAsync(context));
        }

        private async Task AfterDialogEnded(IDialogContext context, IAwaitable<object> result)
        {
            // BUG: this actually waits for user to respond. Needs to be proactive.
            await this.StartAsync(context);
        }

        private async Task AfterChoosingLanguageSwitch(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result as string;
            if (response == null)
            {
                await this.StartAsync(context);
                return;
            }

            try
            {
                UserData userData = _userDataRepository.GetUserData(context.Activity.From.Id);
                var translatedResponse = await MessageTranslator.TranslateTextAsync(response);

                if (translatedResponse.Equals(Shared.Yes, StringComparison.InvariantCultureIgnoreCase))
                {
                    string translatedSelfIntroduction =
                        await MessageTranslator.TranslateTextAsync(BotPersonality.BotSelfIntroduction,
                            userData.NativeLanguageIsoCode);

                    var introductionResponseTask = context.PostAsync($"{translatedSelfIntroduction}");
                    var translatedUserNameQuestionTask =
                        MessageTranslator.TranslateTextAsync(BotPersonality.UserNameQuestion,
                            userData.NativeLanguageIsoCode);
                    await Task.WhenAll(introductionResponseTask, translatedUserNameQuestionTask);

                    await context.PostAsync(translatedUserNameQuestionTask.Result);

                    context.Wait(this.UserNameReceivedInNativeLanguageAsync);
                }
                else
                {
                    await context.PostAsync($"{BotPersonality.UserNameQuestion}");
                    context.Wait(this.UserNameReceivedAsync);
                }
            }
            catch (TooManyAttemptsException)
            {
                UserData userData = _userDataRepository.GetUserData(context.Activity.From.Id);

                string translatedTooManyAttemptMessage = await MessageTranslator.TranslateTextAsync(Shared.TooManyAttemptMessage, userData.NativeLanguageIsoCode);
                await Task.WhenAll(context.PostAsync($"{translatedTooManyAttemptMessage}"),
                    this.StartAsync(context));
            }
        }
    }
}