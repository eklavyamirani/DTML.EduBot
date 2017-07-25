namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.Common;
    using DTML.EduBot.UserData;

    [Serializable]
    public class RootDialog : IDialog<string>
    {
        private readonly int MAX_ATTEMPT = 2;

        private const string Yes = "Yes";
        private const string No = "No";

        private static readonly IEnumerable<string> YesNoChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Yes,
                No});

        private const string ChatWithBot = "Chat With Bot";
        private const string StartTheLessonPlan = "Start English Lesson Plan";

        private static readonly IEnumerable<string> DialogChoices = new ReadOnlyCollection<string>
            (new List<String> {
                ChatWithBot,
                StartTheLessonPlan});

        private const string TooManyAttemptMessage = "Sorry, you have attempted too many times :(";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userText = (await result).Text;
            
            string detectedLanguageIsoCode = await MessageTranslator.IdentifyLangAsync(userText);

            using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
            {
                UserData.UserData userData = new UserData.UserData();
                userData.UserId = context.Activity.From.Id;
                userData.NativeLanguageIsoCode = detectedLanguageIsoCode;

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

                    CultureInfo[] allCultures =CultureInfo.GetCultures(CultureTypes.AllCultures);
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

                    string translatedDontUnderstand = await MessageTranslator.TranslateTextAsync("I am sorry but I didn't understand that. I need you to select one of the options below", detectedLanguageIsoCode);

                    PromptDialog.Choice(
                        context,
                        this.AfterChoosingLanguageSwitch,
                        await this.TranslatedChoices(YesNoChoices, detectedLanguageIsoCode),
                        translatedSwitchQuestion,
                        translatedDontUnderstand,
                        attempts: MAX_ATTEMPT
                    );
                }
            }
        }

        private Task UserNameReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(
                context,
                this.AfterDialogChoiceSelectedAsync,
                DialogChoices,
                $"Hello Dear {result.GetAwaiter().GetResult().Text},\n What would you like to do.",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: MAX_ATTEMPT);

            return Task.CompletedTask;
        }

        private async Task UserNameReceivedInNativeLanguageAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string nativeLanguageIsoCode = MessageTranslator.DEFAULT_LANGUAGE;
            using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
            {
                UserData.UserData userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);
                nativeLanguageIsoCode = userData.NativeLanguageIsoCode;
            }

            string translatedWhatToDo =
                await MessageTranslator.TranslateTextAsync(
                    $"Hello Dear {result.GetAwaiter().GetResult().Text},\n What would you like to do.", nativeLanguageIsoCode);

            string translatedNotUnderstandSelection =
                await MessageTranslator.TranslateTextAsync(
                    "I am sorry but I didn't understand that. I need you to select one of the options below", nativeLanguageIsoCode);

            PromptDialog.Choice(
                context,
                this.AfterDialogChoiceSelectedInNativeLanguageAsync,
                await this.TranslatedChoices(DialogChoices, nativeLanguageIsoCode),
                translatedWhatToDo,
                translatedNotUnderstandSelection,
                attempts: MAX_ATTEMPT);
        }

        private async Task AfterDialogChoiceSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                // TODO: enum.
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    // TODO: enum.
                    switch (selection)
                    {
                        case ChatWithBot:
                            await context.PostAsync("Great! Say Hello, and see what will I respond!");
                            context.Call(scope.Resolve<ChitChatDialog>(), this.AfterDialogEnded);
                            break;

                        case StartTheLessonPlan:
                            context.Call(scope.Resolve<LessonPlanDialog>(), this.AfterDialogEnded);
                            break;
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
                await Conversation.SendAsync(context.MakeMessage(), () => this);
            }
        }

        private async Task AfterDialogChoiceSelectedInNativeLanguageAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    UserData.UserData userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);
                    string nativeLanguageIsoCode = userData.NativeLanguageIsoCode;

                    string translatedChatWithBot = await MessageTranslator.TranslateTextAsync(ChatWithBot,
                        nativeLanguageIsoCode);
                    string translatedStartTheLessonPlan = await MessageTranslator.TranslateTextAsync(StartTheLessonPlan,
                        nativeLanguageIsoCode);

                    if (translatedChatWithBot.Equals(selection, StringComparison.OrdinalIgnoreCase))
                    {
                        await context.PostAsync("Great! We will now start conversation in English! Excited?");
                        context.Call(scope.Resolve<ChitChatDialog>(), this.AfterDialogEnded);
                    }
                    else if (translatedStartTheLessonPlan.Equals(selection, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Call(scope.Resolve<LessonPlanDialog>(), this.AfterDialogEnded);
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
                await Conversation.SendAsync(context.MakeMessage(), () => this);
            }
        }

        private async Task AfterDialogEnded(IDialogContext context, IAwaitable<object> result)
        {
            await Conversation.SendAsync(context.MakeMessage(), () => this);
        }

        private async Task AfterChoosingLanguageSwitch(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result as string;

            try
            {
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    UserData.UserData userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);

                    string translatedYes = await MessageTranslator.TranslateTextAsync(Yes, userData.NativeLanguageIsoCode);
                    string translatedNo = await MessageTranslator.TranslateTextAsync(No, userData.NativeLanguageIsoCode);

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
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    UserData.UserData userData =
                        scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);
                    
                    string translatedTooManyAttemptMessage = await MessageTranslator.TranslateTextAsync(TooManyAttemptMessage, userData.NativeLanguageIsoCode);

                    await context.PostAsync($"{translatedTooManyAttemptMessage}");
                }
                await Conversation.SendAsync(context.MakeMessage(), () => this);
            }
        }

        private async Task<ICollection<string>> TranslatedChoices(IEnumerable<string> choices, string languageToTranslate)
        {
            ICollection<string> translatedChoices = new Collection<string>();

            foreach (string choice in choices)
            {
                string translatedChoice = await MessageTranslator.TranslateTextAsync(choice, languageToTranslate);
                translatedChoices.Add(translatedChoice);
            }

            return translatedChoices;
        }
    }
}