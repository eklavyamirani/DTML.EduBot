using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
namespace DTML.EduBot.Dialogs
{
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Constants;
    using DTML.EduBot.Common;
    using DTML.EduBot.Extensions;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {

        [LuisIntent("BotQuestions")]
        public async Task HandleWhatIsIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Name))
            {
                await context.PostTranslatedAsync($"My name is {BotPersonality.BotName}");
            }
            else
            {
                await context.PostTranslatedAsync(BotPersonality.GetRandomGenericResponse());
            }
        }
    }
}