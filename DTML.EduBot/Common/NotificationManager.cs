using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace DTML.EduBot.Common
{
    public class NotificationManager : INotificationManager
    {
        string url = "https://dtml.org/Activity/RecordUserActivityServerToServer/?apiKey={0}&token={1}&userName={2}&source=bot&eventMessage={3}&eventType={4}&eventData={5}";
        public void RecordEvent(string eventType, string eventLabel, string evenMessage, string eventUser)
        {
            try
            {
                var api = ConfigurationManager.AppSettings["DTMLAppKey"];
                var token = ConfigurationManager.AppSettings["MicrosoftAppPassword"];
                var callURL = string.Format(url, api, token, eventUser, evenMessage, eventType, eventLabel);

                using (WebClient wc = new WebClient())
                {
                    Stream data = wc.OpenRead(callURL);
                    StreamReader reader = new StreamReader(data);
                    string s = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                    Console.WriteLine("Success");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public enum EventType
    {
        SystemEvent = 0,
        GameLoaded,
        GameRated,
        GameCompleted,
        Conversation,
        EngagementClosed,
        ELearningTimeRecorded,
        LevelCompleted,
    }
}