namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;
    using DTML.EduBot.Qna;

    [LuisModel("31511772-4f1c-4590-87a8-0d6b8a7707a1", "a88bd2b022e34d5db56a73eb2bd33726")]
    [QnaModel(hostUri: "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/",
       subscriptionKey: "d24b3b5df8b541cabfab6d4b12646ca0",
       modelId: "34aeee3d-51f6-42d4-bcb3-b8e8c1c1b88e")]
    [Serializable]
    public partial class RootDialog : QnaLuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            await ConvertAndPostResponse(context, BotPersonality.BotResponseUnrecognizedIntent);
        }

       [LuisIntent("Gibberish")]
        public async Task HandleGibberish(IDialogContext context, LuisResult result)
        {
            await ConvertAndPostResponse(context, BotPersonality.BotResponseToGibberish);
        }

       
    }
}