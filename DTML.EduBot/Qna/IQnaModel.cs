namespace DTML.EduBot.Qna
{
    using System;

    public interface IQnaModel
    {
        string SubscriptionKey { get; }
        string ModelId { get; }
        string HostUri { get; }
        Uri BuildUri();
    }
}
