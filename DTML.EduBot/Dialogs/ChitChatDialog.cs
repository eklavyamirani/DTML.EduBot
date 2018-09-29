namespace DTML.EduBot.Dialogs
{
    using Attributes;
    using Autofac.Integration.WebApi;
    using Common;
    using DTML.EduBot.Constants;
    using DTML.EduBot.UserData;
    using Extensions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using static DTML.EduBot.Common.AzureTableLogger;

    [PreConfiguredLuisModel]
    [PreConfiguredQnaModel]
    [Serializable]
    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        private static readonly IReadOnlyCollection<string> YesNoChoices = new ReadOnlyCollection<string>
    (new List<String> {
                Shared.Yes,
                Shared.No});

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            var userData = context.GetUserData();
            var detectedLanguage = await MessageTranslator.IdentifyLangAsync(result.Query);

            if (!detectedLanguage.Equals(userData.NativeLanguageIsoCode) && !detectedLanguage.Equals(MessageTranslator.DEFAULT_LANGUAGE))
            {
                context.UserData.SetValue(Constants.Shared.UserLanguageCodeKey, detectedLanguage);
                await SwitchLanguage(context, userData, detectedLanguage);
            }
            else
            {
                await context.PostTranslatedAsync(BotPersonality.BotResponseUnrecognizedIntent);
                await EngageWithUser(context);
            }
        }

        private async Task SwitchLanguage(IDialogContext context, UserData userData, string locale)
        {
            var detectedLanguage = context.UserData.GetValue<string>(Constants.Shared.UserLanguageCodeKey);
            CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            CultureInfo detectedCulture =
               allCultures
                    .FirstOrDefault(cultureInfo => (detectedLanguage.Contains(cultureInfo.TwoLetterISOLanguageName)));

            string detectedLanguageName = CultureInfo.GetCultureInfo(MessageTranslator.DEFAULT_LANGUAGE).DisplayName;

            if (detectedCulture != null)
            {
                detectedLanguageName = detectedCulture.DisplayName;
            }

            var translationInputTextlist = new List<string>(capacity: 4);
            translationInputTextlist.AddRange(YesNoChoices);
            translationInputTextlist.AddRange(new string[] { $"Do you want to switch to {detectedLanguageName}", Shared.DoNotUnderstand });

            var translatedList = await MessageTranslator.TranslateTextAsync(translationInputTextlist, locale);
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
                var detectedLanguage = context.UserData.GetValue<string>(Constants.Shared.UserLanguageCodeKey);
                UserData userData = context.GetUserData();
                var translatedResponse = await MessageTranslator.TranslateTextAsync(response);

                if (translatedResponse.Equals(Shared.Yes, StringComparison.InvariantCultureIgnoreCase))
                {
                    userData.NativeLanguageIsoCode = detectedLanguage;
                    var englishName = CultureInfo.GetCultureInfo(userData.NativeLanguageIsoCode).EnglishName;

                    string translatedSelfIntroduction =
                        await MessageTranslator.TranslateTextAsync(string.Format(BotPersonality.BotLanguageIntroduction, englishName), userData.NativeLanguageIsoCode);

                    var introductionResponseTask = context.PostTranslatedAsync($"{translatedSelfIntroduction}");
                    var translatedUserNameQuestionTask =
                        MessageTranslator.TranslateTextAsync(BotPersonality.UserNameQuestion,
                            userData.NativeLanguageIsoCode);
                    await Task.WhenAll(introductionResponseTask, translatedUserNameQuestionTask);

                    await context.PostTranslatedAsync(translatedUserNameQuestionTask.Result);

                    context.UpdateUserData(userData);
                    await EngageWithUser(context);
                }

            }
            catch (TooManyAttemptsException)
            {
                UserData userData = context.GetUserData();

                string translatedTooManyAttemptMessage = await MessageTranslator.TranslateTextAsync(Shared.TooManyAttemptMessage, userData.NativeLanguageIsoCode);
                await Task.WhenAll(context.PostTranslatedAsync($"{translatedTooManyAttemptMessage}"),
                    this.StartAsync(context));
            }
        }

    }
}