namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;
    using DTML.EduBot.Qna;
    using Microsoft.Azure;
    using Attributes;
    using System.Linq;
    using DTML.EduBot.Constants;
    using DTML.EduBot.Common.Interfaces;

    [PreConfiguredLuisModel]
    [PreConfiguredQnaModel]
    [Serializable]
    public partial class NavigateDialog : QnaLuisDialog<object>
    {
        public NavigateDialog(ILogger logger) : base(logger)
        {
        }

        [LuisIntent("Navigate")]
        public async Task HandleNavigateIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Next))
            {
                //TODO: move to next item
                await context.PostAsync("We can move on if that's what you want");
            }
            else if (result.Entities.Any(e => e.Type == BotEntities.Previous))
            {
                //TODO: move to previous item
                await context.PostAsync("We can go back if that's what you want");
            }
            else if (result.Entities.Any(e => e.Type == BotEntities.StartOver))
            {
                //TODO: start over current item
                await context.PostAsync("We can start over if that's what you want");
            }
            else
            {
                await context.PostAsync("Did you want to go somewhere?");
            }
        }

    }
}