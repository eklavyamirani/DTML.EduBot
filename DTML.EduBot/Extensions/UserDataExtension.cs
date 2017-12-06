using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs;
using DTML.EduBot.Common;
using DTML.EduBot.Constants;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace DTML.EduBot.UserData
{
    public static class UserDataExtension
    {
        private const string UserDataRepositoryKey = "UserDataRepositoryKey";
        public static void UpdateUserData(this IDialogContext context, UserData userData)
        {
            if (userData != null)
            {
                try
                {
                    context.PrivateConversationData.SetValue<UserData>(UserDataRepositoryKey, userData);
                }
                catch (Exception)
                { }
            }
        }

        public static UserData GetUserData(this IDialogContext context)
        {
            try
            {
                UserData userData = null;
                string userName = context.UserData.ContainsKey(Shared.UserName) ? context.UserData.GetValue<string>(Shared.UserName) : string.Empty;

                if (!context.PrivateConversationData.TryGetValue(UserDataRepositoryKey, out userData))
                {
                    if (userData == null)
                    {
                        userData = new UserData();
                        userData.UserName = userName;
                        userData.UserId = context.Activity.From.Id;
                        userData.NativeLanguageIsoCode = MessageTranslator.DEFAULT_LANGUAGE;
                        context.UpdateUserData(userData);
                    }
                }
            }
            catch
            { }

            return new UserData();
        }
    }
}