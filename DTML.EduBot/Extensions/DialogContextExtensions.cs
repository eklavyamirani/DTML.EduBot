namespace DTML.EduBot.Extensions
{
    using Microsoft.Bot.Builder.Dialogs;
    using System.Threading.Tasks;
    using DTML.EduBot.Common;
    using Microsoft.Bot.Connector;
    using System;

    public static class LocalizationExtensions
    {
        public static Random rnd = new Random();

        public static async Task PostTypingAsync(this IDialogContext context, int delay = 1000)
        {
            try
            {
                var typingMessage = context.MakeMessage();
                typingMessage.Type = ActivityTypes.Typing;
                typingMessage.Text = "";
                await context.PostAsync(typingMessage);
                await Task.Delay(delay);
                return;
            }
            catch (Exception ex)
            { }
        }

        public static async Task PostTranslatedAsync(this IDialogContext context, string message, bool sendTyping = true, int delay = 0)
        {
            if (sendTyping)
            {
                if (delay == 0)
                {
                    //generate random delay from 1 second to 4 seconds...
                    delay = rnd.Next(1, 4) * 1000;
                }

                await PostTypingAsync(context, delay);
            }

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