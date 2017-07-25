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
    using DTML.EduBot.Constants;
    using DTML.EduBot.UserData;

    [Serializable]
    public class RootDialog : IDialog<string>
    {

        private static readonly IEnumerable<string> YesNoChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.Yes,
                Shared.No});
        
        private static readonly IEnumerable<string> DialogChoices = new ReadOnlyCollection<string>
            (new List<String> {
                Shared.ChatWithBot,
                Shared.StartTheLessonPlan});

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
                UserData userData = new UserData();
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

            PromptDialog.Choice(
                context,
                this.AfterDialogChoiceSelectedAsync,
                DialogChoices,
                $"Hello Dear {userText},\n What would you like to do.",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: Shared.MaxAttempt);
        }

        private async Task UserNameReceivedInNativeLanguageAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string nativeLanguageIsoCode = MessageTranslator.DEFAULT_LANGUAGE;
            using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
            {
                UserData userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);
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
                await MessageTranslator.TranslatedChoices(DialogChoices, nativeLanguageIsoCode),
                translatedWhatToDo,
                translatedNotUnderstandSelection,
                attempts: Shared.MaxAttempt);
        }

        private async Task AfterDialogChoiceSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    switch (selection)
                    {
                        case Shared.ChatWithBot:
                            await context.PostAsync("Great! Say Hello, and see what will I respond!");
                            context.Call(scope.Resolve<ChitChatDialog>(), this.AfterDialogEnded);
                            break;

                        case Shared.StartTheLessonPlan:
                            context.Call(scope.Resolve<LessonPlanDialog>(), this.AfterDialogEnded);
                            break;
                    }
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterDialogChoiceSelectedInNativeLanguageAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    UserData userData = scope.Resolve<IUserDataRepository>().GetUserData(context.Activity.From.Id);
                    string nativeLanguageIsoCode = userData.NativeLanguageIsoCode;

                    string translatedChatWithBot = await MessageTranslator.TranslateTextAsync(Shared.ChatWithBot,
                        nativeLanguageIsoCode);
                    string translatedStartTheLessonPlan = await MessageTranslator.TranslateTextAsync(Shared.StartTheLessonPlan,
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
                await this.StartAsync(context);
            }
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
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
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
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
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