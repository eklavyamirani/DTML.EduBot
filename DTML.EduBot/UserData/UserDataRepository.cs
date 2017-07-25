using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace DTML.EduBot.UserData
{
    public class UserDataRepository : IUserDataRepository
    {
        // TODO persist in database instead in memory
        private IDictionary<string, UserData> userDataStore =
            new ConcurrentDictionary<string, UserData>(Environment.ProcessorCount * 2, 101);

        public void UpdateUserData(UserData userData)
        {
            if (GetUserData(userData.UserId) == null)
            {
                userDataStore.Add(userData.UserId, userData);
            }
            else
            {
                userDataStore.Remove(userData.UserId);
                userDataStore.Add(userData.UserId, userData);
            }
        }

        public UserData GetUserData(string userId)
        {
            UserData userData = null;

            if (!userDataStore.TryGetValue(userId, out userData))
            {
                return null;
            }

            return userData;
        }
    }
}