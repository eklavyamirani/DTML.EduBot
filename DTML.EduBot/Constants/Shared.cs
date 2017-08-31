using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

namespace DTML.EduBot.Constants
{
    public class Shared
    {
        public const int MaxPromptAttempts = 2;

        public const string ClientNewLine = "<br/>";
        public const string NoMoreLessonsMessage = "That's all the lessons for today. Have a nice rest of your day!";
        public const string NoMoreQuizesMessage = "That's all the quiz questions for today.";

        public const string Yes = "Yes";
        public const string No = "No";

        public const string ChatWithBot = "Chat With Bot";
        public const string StartTheLessonPlan = "Start English Lesson Plan";

        public const string TooManyAttemptMessage = "Sorry, you have attempted too many times :(";
        public const string RepeatAfterMe = "Hear and repeat the phrase below:";
        public const string CorrectAnswerMessage = "That is correct.!";
        public const string TopicCompleteMessage = "WOW! You finished the whole topic! ";
        public const string LessonCompleteMessage = "This is the end of the current lesson. What would you like to do next?";
        public const string AllTopicsCompleteMessage = "You completed all the topics. Congratulations!";
        public const string DoNotUnderstand = "I am sorry but I didn't understand that. I need you to select one of the options below";
        public const string ReadyForQuiz = "Are you ready for the quiz?";

        public const string LevelOne = "1";
        public const string LevelTwo = "2";
        public const string LevelThree = "3";
        public const string LevelFour = "4";
        public const string LevelFive = "5";
        public const string UserLanguageCodeKey = "UserLanguageCode";
    }
}