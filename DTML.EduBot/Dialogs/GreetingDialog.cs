namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string botresponse = BotPersonality.GetRandomGreeting();
            string translatedBotResponse = await this.TranslateBotResponseAsync(context, botresponse);
            await context.PostAsync(translatedBotResponse);
        }

        [LuisIntent("Courtesy")]
        public async Task Courtesy(IDialogContext context, LuisResult result)
        {
            string translatedBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.GetRandomGenericResponse());

            await context.PostAsync(translatedBotResponse);
            await EngageWithUser(context);
        }
    }
}