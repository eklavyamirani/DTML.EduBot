namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DTML.EduBot.Qna;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using EduBot.Common;

    [Serializable]
    public abstract class QnaLuisDialog<TResult> : LuisDialog<TResult>
    {
        protected readonly QnaService qnaService;
        protected const int QNA_THRESHOLD = 0;

        public QnaLuisDialog()
        {
            qnaService = MakeQnaServiceFromAttributes();
        }

        private QnaService MakeQnaServiceFromAttributes()
        {
            var type = this.GetType();
            var qnaModel = type.GetCustomAttributes(typeof(QnaModelAttribute), false)
                .FirstOrDefault() as QnaModelAttribute;

            return qnaModel == null ? null : new QnaService(qnaModel);
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            if (qnaService == null)
            {
                await base.MessageReceived(context, item).ConfigureAwait(false);
                return;
            }

            var message = await item;
            IQnaResult qnaResult;
            try
            {
                qnaResult = await qnaService.QueryAsync(message.Text, context.CancellationToken).ConfigureAwait(false);
                if (qnaResult.Score > QNA_THRESHOLD)
                {
                    await QnaHandler(context, qnaResult).ConfigureAwait(false);
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // TODO: Log exception
            }

            await base.MessageReceived(context, item).ConfigureAwait(false);
        }

        protected virtual async Task QnaHandler(IDialogContext context, IQnaResult result)
        {
            await ConvertAndPostResponse(context, result.Answer);
        }

        protected static async Task ConvertAndPostResponse(IDialogContext context, string result)
        {
            string translatedText = await MessageTranslator.TranslateTextAsync(result);
            if (translatedText == null)
            {
                translatedText = $"Sorry I did not get that. Please repeat.";
            }

            await context.PostAsync(translatedText);
        }
    }
}