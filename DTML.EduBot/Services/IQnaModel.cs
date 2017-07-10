namespace DTML.EduBot.Services
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
