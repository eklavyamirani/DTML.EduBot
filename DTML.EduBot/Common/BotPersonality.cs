using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DTML.EduBot.Constants;

namespace DTML.EduBot.Common
{
    public class BotPersonality
    {
        private static int Index = 0;
        public const string BotSelfIntroduction = "Hi I am " + BotName;
        public const string BotName = "Professor Edword Jr.";
        public const string UserNameQuestion = "What is your name?";
        public const string BotResponseUnrecognizedIntent = "I'm still learning just as you are, I will think and get back to you";
        public const string BotResponseToGibberish = "Hmm, that doesn't sound right, can you please rephrase?";
        public const string BotResponseToUserName = "That's a nice name";
        public const string BotResponseToDeclinedLessonPlan = "Aww!Okay let's keep chatting";

        private static readonly Random random = new Random();

        private readonly static IReadOnlyList<string> BotGreeting= new List<string>
            {
                "Hello, Its nice to meet you, \n How are you doing today?",
                "Hello there my friend, \n How is your day so far?",
                "Hi buddy, good to see you \n How are you doing today?",
                "Long time, I missed you, \n How have you been?",
                "Good to see you my friend, \n How are you doing?"
            };

         private readonly static IReadOnlyList<string> AcquaintanceQs = new List<string>
            {
                "Which is your favourite animal?",
                "What is your favorite color?",
                "What is your favorite food?", 
                "What did you eat today?",
                "What did you study today?"
            };

        private readonly static IReadOnlyList<string> StartLessonPlanQuestions = new List<string>
            {
                "How about an English lesson?",
                "Shall we do an English exercise now?",
                "Would you like to improve your English with a lesson?",
                "Would you like to learn some English now?",
            };

        private readonly static IReadOnlyList<string> GenericResponses = new List<string>
            {
                "Great!", "Good!", "Awesome!!",
                "Very good","Perfect!", "Interesting!",
                "Amazing", "Excellent!", "Super"
            };


        public static string GetRandomGreeting()
        {
            int RandIndex = random.Next(BotGreeting.Count);
            return (BotGreeting[RandIndex]);
        }

        public static string BuildAcquaintance()
        {
            if (Index > AcquaintanceQs.Count - 1)
            {
                Index = 0;
                return "You are an interesting person!" + Shared.ClientNewLine + "Do you have any questions for me?";
            }
            else
            {
                return AcquaintanceQs[Index++];
            }

        }

        public static string GetRandomGenericResponse()
        {
            int randIndex = random.Next(GenericResponses.Count);
            return (GenericResponses[randIndex]);
        }

        public static string GetStartLessonPlanQuestion()
        {
            int randIndex = random.Next(StartLessonPlanQuestions.Count);
            return (StartLessonPlanQuestions[randIndex]);
        }
    }
}