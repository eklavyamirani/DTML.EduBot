namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Constants;
    using Extensions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using System.Linq;

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
            else if (result.Entities.Any(e => e.Type == BotEntities.Hobby))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Animal))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Place))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Language))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Family))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Color))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Subject))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Food))
            {

            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Drink))
            {

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