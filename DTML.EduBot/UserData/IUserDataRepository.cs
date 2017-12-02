using Microsoft.Bot.Builder.Dialogs;

namespace DTML.EduBot.UserData
{
    public interface IUserDataRepository
    {
        void UpdateUserData(UserData userData, IDialogContext context);

       UserData GetUserData(IDialogContext context);
    }
}