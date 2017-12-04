using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs;
using DTML.EduBot.Common;
using DTML.EduBot.Constants;

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
                try
                {
                    context.UserData.SetValue<UserData>(UserDataRepositoryKey, userData);
                }
                catch (Exception ex)
                { }
            }
        }

        public UserData GetUserData(IDialogContext context)
        {
            try
            {
                UserData userData = null;
                string userName = context.UserData.ContainsKey(Shared.UserName) ? context.UserData.GetValue<string>(Shared.UserName) : string.Empty;

                if (!context.UserData.TryGetValue(UserDataRepositoryKey, out userData))
                {
                    if (userData == null)
                    {
                        userData = new UserData();
                        userData.UserName = userName;
                        userData.UserId = context.Activity.From.Id;
                        userData.NativeLanguageIsoCode = MessageTranslator.DEFAULT_LANGUAGE;
                        this.UpdateUserData(userData, context);
                    }
                }
            }
            catch
            { }

            return new UserData();
        }
    }
}