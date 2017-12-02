using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs;

namespace DTML.EduBot.UserData
{
    [Serializable]
    public class UserDataRepository : IUserDataRepository
    {
        private const string UserDataRepositoryKey = "UserDataRepositoryKey";
        public void UpdateUserData(UserData userData, IDialogContext context)
        {
            if (userData != null)
            {
                context.UserData.SetValue<UserData>(UserDataRepositoryKey, userData);
            }
        }

        public UserData GetUserData(IDialogContext context)
        {
            UserData userData = null;

            if (!context.UserData.TryGetValue(UserDataRepositoryKey, out userData))
            {
                return null;
            }

            return userData;
        }
    }
}