namespace DTML.EduBot.Attributes
{
    using System;
    using DTML.EduBot.Qna;
    using Microsoft.Azure;

    [Serializable]
    public class PreConfiguredQnaModelAttribute : QnaModelAttribute
    {
        public PreConfiguredQnaModelAttribute()
            : base(CloudConfigurationManager.GetSetting("qnaHostServiceEndpoint").ToString(), 
                  CloudConfigurationManager.GetSetting("qnaSubscriptionKey").ToString(),
                  CloudConfigurationManager.GetSetting("qnaModelId").ToString())
        {
        }
    }
}