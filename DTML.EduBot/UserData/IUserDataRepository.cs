namespace DTML.EduBot.UserData
{
    public interface IUserDataRepository
    {
       void UpdateUserData(UserData userData);

       UserData GetUserData(string userId);
    }
}