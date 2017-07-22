namespace DTML.EduBot.Attributes
{
    using System;
    using Microsoft.Azure;
    using Microsoft.Bot.Builder.Luis;

    [Serializable]
    public class PreConfiguredLuisModelAttribute : LuisModelAttribute
    {
        public PreConfiguredLuisModelAttribute()
            : base(CloudConfigurationManager.GetSetting("luisModelId").ToString(), CloudConfigurationManager.GetSetting("luisSubscriptionKey").ToString())
        {
        }
    }
}