using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Common.Interfaces
{
    public interface INotificationManager
    {
        void RecordEvent(string eventType, string EventLabel, string EvenMessage, string EventUser);
    }
}