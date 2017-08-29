namespace DTML.EduBot.Extensions
{
    using Microsoft.Bot.Builder.Dialogs;
    using System.Threading.Tasks;
    using DTML.EduBot.Common;

    public static class LocalizationExtensions
    {
        public static async Task PostTranslatedAsync(this IDialogContext context, string message)
        {
            string language;
            if (context.UserData.TryGetValue(Constants.Shared.UserLanguageCodeKey, out language) && language != MessageTranslator.DEFAULT_LANGUAGE)
            {
                var translatedMessage = await MessageTranslator.TranslateTextAsync(message, language);
                await context.PostAsync(translatedMessage);
                return;
            }
            else
            {
                await context.PostAsync(message);
                return;
            }
        }
    }
}