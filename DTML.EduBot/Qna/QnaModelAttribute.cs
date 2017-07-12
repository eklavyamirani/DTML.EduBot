namespace DTML.EduBot.Qna
{
    using System;
    using DTML.EduBot.Extensions;

    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class QnaModelAttribute : Attribute, IQnaModel
    {
        private const string QnaUriFormat = "{0}/knowledgebases/{1}/generateAnswer";
        private string subscriptionKey;
        private string modelId;
        private string hostUri;

        public string SubscriptionKey
        {
            get
            {
                return subscriptionKey;
            }
            private set
            {
                subscriptionKey = value;
            }
        }

        public string ModelId
        {
            get
            {
                return modelId;
            }
            private set
            {
                modelId = value;
            }
        }

        public string HostUri
        {
            get
            {
                return hostUri;
            }
            private set
            {
                hostUri = value;
            }
        }

        public QnaModelAttribute(string hostUri, string subscriptionKey, string modelId)
        {
            Set.FieldNotNull(hostUri, nameof(hostUri), out this.hostUri);
            Set.FieldNotNull(subscriptionKey, nameof(subscriptionKey), out this.subscriptionKey);
            Set.FieldNotNull(modelId, nameof(modelId), out this.modelId);
        }


        public Uri BuildUri()
        {
            return new Uri(string.Format(QnaUriFormat, HostUri, modelId));
        }
    }
}