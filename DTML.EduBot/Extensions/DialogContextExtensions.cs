namespace DTML.EduBot.Extensions
{
    using Microsoft.Bot.Builder.Dialogs;
    using System.Threading.Tasks;
    using DTML.EduBot.Common;
    using DTML.EduBot.Helpers;
    using Microsoft.Bot.Connector;
    using System.Threading;
    using System;

    public static class LocalizationExtensions
    {
        public static async Task PostTranslatedAsync(this IDialogContext context, string message)
        {
            string language;
            if (context.UserData.TryGetValue(Constants.Shared.UserLanguageCodeKey, out language) && language != MessageTranslator.DEFAULT_LANGUAGE)
            {
                var translatedMessage = await MessageTranslator.TranslateTextAsync(message, language);
                await context.PostLogAsync(translatedMessage);
                return;
            }
            else
            {
                await context.PostLogAsync(message);
                return;
            }
        }

        public static async Task PostLogAsync(this IDialogContext context, string msg = null, IMessageActivity botMessage = null)
        {
            if (botMessage == null)
            {
                var userMessage = context.Activity as IMessageActivity;
                botMessage = context.MakeMessage();
                botMessage.Text = msg;
                await ConversationHelper.LogMessage(botMessage, userMessage, true);
                await context.PostAsync(botMessage, CancellationToken.None);
            }
            else
            {
                var userMessage = context.Activity as IMessageActivity;
                await ConversationHelper.LogMessage(botMessage, userMessage, true);
                await context.PostAsync(botMessage, CancellationToken.None);
            }
        }

        public static async Task DoneLogAsync(this IDialogContext context, string msg = null, IMessageActivity botMessage = null)
        {
            if (botMessage == null)
            {
                var userMessage = context.Activity as IMessageActivity;
                botMessage = context.MakeMessage();
                botMessage.Text = msg;
                await ConversationHelper.LogMessage(botMessage, userMessage, true);
                context.Done(botMessage);
            }
            else
            {
                var userMessage = context.Activity as IMessageActivity;
                await ConversationHelper.LogMessage(botMessage, userMessage, true);
                context.Done(botMessage);
            }
        }
    }
}