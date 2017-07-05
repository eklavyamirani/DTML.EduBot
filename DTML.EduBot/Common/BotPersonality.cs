using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Common
{
    public class BotPersonality
    {
        public const string BotName = "Professor Edword";
        private static int Index = 0;
        public const string BotResponseUnrecognizedIntent = "I'm still learning just as you are, I will think and get back to you";
        public const string BotResponseToGibberish = "Hmm, that doesn't sound right, can you please rephrase?";
        public const string BotResponseToUserName = "That's a nice name";

        private readonly static IReadOnlyList<string> BotGreeting= new List<string>
            {
                "Hello, Its nice to meet you, \n How are you doing today?",
                "Hello there my friend, \n How is your day so far?",
                "Hi buddy, good to see you \n How are you doing today?",
                "Long time, I missed you, \n How have you been?",
                "Good to see you my friend, \n How are you doing?"
            };

         private readonly static IReadOnlyList<string> ChatContinuer = new List<string>
            {
                "What is your name?",
                "In which class are you studying?",
                "What is your favorite color?",
                "What is your favorite food?", 
                "What did you eat today?",
                "What did you study today?"
            };

        private readonly static IReadOnlyList<string> GenericResponses = new List<string>
            {
                "Great!", "Good!", "Awesome!!",
                "Very good","Perfect!", "Interesting!",
                "Amazing", "Excellent!", "Super"
            };


        public static string GetRandomGreeting()
        {
            Random RandomNum = new Random();
            int RandIndex = RandomNum.Next(BotGreeting.Count);
            return (BotGreeting[RandIndex]);
        }

        public static string GetChatContinuer()
        {
            if (Index > ChatContinuer.Count - 1)
            {
                Index = 0;
                return "I do not know what to say";
            }
            else
            {
                return ChatContinuer[Index++];
            }

        }

        public static string GetRandomGenericResponse()
        {
            Random randomNum = new Random();
            int randIndex = randomNum.Next(GenericResponses.Count);
            return (GenericResponses[randIndex]);
        }
    }
}