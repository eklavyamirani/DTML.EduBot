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
        public const string BotSelfIntroductionStart = "Hi I am " + BotName+", ask me any question or type 'learn english' to start learning plan";
        public const string BotLanguageIntroduction = "My {0} is not very good, but I wil try...";
        public const string BotName = "Professor Edword Jr.";
        public const string UserNameQuestion = "What is your name?";
        public const string BotResponseUnrecognizedIntent = "I'm still learning just as you are, this is not something I know...";
        public const string BotResponseToGibberish = "Hmm, that doesn't sound right, can you please rephrase?";
        public const string BotResponseToUserName = "That's a nice name";
        public const string BotResponseToDeclinedLessonPlan = "Aww! Okay let's keep chatting";

        private readonly static IReadOnlyList<string> BotGreeting= new List<string>
            {
                "Hello, Its nice to meet you, \n How are you doing today?",
                "Hello there my friend, \n How is your day so far?",
                "Hi buddy, good to see you \n How are you doing today?",
                "Long time, I missed you, \n How have you been?",
                "Good to see you my friend, \n How are you doing?"
            };

        private readonly static IReadOnlyList<string> BotGoodbye = new List<string>
            {
               "Goodbye now!\n",
               "Talk to you soon!\n",
               "It was nice talking to you! See you later!\n",
               "It was nice see you again! Goodbye!\n",
               "Talk to you again!\n"
            };

        private readonly static IReadOnlyList<string> StartLesson = new List<string>
            {
               "Let's get started on your lessons!\n",
               "Let's begin your lessons!\n",
               "So you want to learn English? Let's begin.\n",
               "Lesson Time. Let's get started.\n"
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
            int RandIndex = (int) (DateTime.UtcNow.Ticks % BotGreeting.Count);
            return (BotGreeting[RandIndex]);
        }

        internal static string GetRandomGoodbye()
        {
            Random RandomNum = new Random();
            int RandIndex = RandomNum.Next(BotGoodbye.Count);
            return (BotGoodbye[RandIndex]);
        }

        internal static string GetRandomStartLesson()
        {
            Random RandomNum = new Random();
            int RandIndex = RandomNum.Next(StartLesson.Count);
            return (StartLesson[RandIndex]);
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
            int randIndex = (int)(DateTime.UtcNow.Ticks % GenericResponses.Count);
            return (GenericResponses[randIndex]);
        }

        public static string GetStartLessonPlanQuestion()
        {
            int randIndex = (int)(DateTime.UtcNow.Ticks % StartLessonPlanQuestions.Count);
            return (StartLessonPlanQuestions[randIndex]);
        }
    }
}