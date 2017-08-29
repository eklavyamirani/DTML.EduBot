using System;
namespace DTML.EduBot.Dialogs
{
    using System.Threading.Tasks;
    using Common;
    using Constants;
    using Extensions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        private const int InitiateLessonPlan = 0;
        private readonly Random random = new Random();

        [LuisIntent("UserInfo")]
        public async Task HandleUserInformation(IDialogContext context, LuisResult result)
        {
            EntityRecommendation entity;
            string BotResponse = BotPersonality.GetRandomGenericResponse() + Shared.ClientNewLine;
            if (result.TryFindEntity(BotEntities.Name, out entity))
            {
                ChatContext.Username = entity.Entity;
                BotResponse = BotPersonality.BotResponseToUserName + " " + ChatContext.Username + "! " + Shared.ClientNewLine;
            }
            else
            {
                BotResponse = BotPersonality.GetRandomGenericResponse();
            }

            await context.PostTranslatedAsync(BotResponse);
            await EngageWithUser(context);
        }

        private async Task EngageWithUser(IDialogContext context)
        {
            if (random.Next(2) == InitiateLessonPlan)
            {
                await AskToStartLessonPlan(context);
            }
            else
            {
                await context.PostTranslatedAsync(BotPersonality.BuildAcquaintance());
            }
        }
    }
}