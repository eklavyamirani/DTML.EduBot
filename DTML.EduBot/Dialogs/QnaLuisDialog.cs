namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DTML.EduBot.Qna;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;

    [Serializable]
    public abstract class QnaLuisDialog<TResult> : LuisDialog<TResult>
    {
        protected readonly QnaService qnaService;
        protected const int QNA_THRESHOLD = 80;

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

        protected virtual async Task QnaHandler(IDialogContext context, IQnaResult result)
        {
            await context.PostAsync(result.Answer);
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var messageText = await GetLuisQueryTextAsync(context, message);

            if (messageText != null)
            {
                // Modify request by the service to add attributes and then by the dialog to reflect the particular query
                var tasks = this.services.Select(s => s.QueryAsync(ModifyLuisRequest(s.ModifyRequest(new LuisRequest(messageText))), context.CancellationToken)).ToArray();
                var qnaTask = qnaService.QueryAsync(message.Text, context.CancellationToken);
                var results = Task.WhenAll(tasks);

                await Task.WhenAll(results, qnaTask);
                var qnaResult = qnaTask.Result;
                var winners = from result in results.Result.Select((value, index) => new { value, index })
                              let resultWinner = BestIntentFrom(result.value)
                              where resultWinner != null
                              select new LuisServiceResult(result.value, resultWinner, this.services[result.index]);

                var winner = this.BestResultFrom(winners);
                if (qnaResult.Score > QNA_THRESHOLD && qnaResult.Score > winner.BestIntent.Score)
                {
                    await QnaHandler(context, qnaResult);
                    return;
                }

                if (winner == null)
                {
                    throw new InvalidOperationException("No winning intent selected from Luis results.");
                }

                if (winner.Result.Dialog?.Status == DialogResponse.DialogStatus.Question)
                {
#pragma warning disable CS0618
                    var childDialog = await MakeLuisActionDialog(winner.LuisService,
                                                                 winner.Result.Dialog.ContextId,
                                                                 winner.Result.Dialog.Prompt);
#pragma warning restore CS0618
                    context.Call(childDialog, LuisActionDialogFinished);
                }
                else
                {
                    await DispatchToIntentHandler(context, item, winner.BestIntent, winner.Result);
                }
            }
            else
            {
                var intent = new IntentRecommendation() { Intent = string.Empty, Score = 1.0 };
                var result = new LuisResult() { TopScoringIntent = intent };
                await DispatchToIntentHandler(context, item, intent, result);
            }
        }
    }
}