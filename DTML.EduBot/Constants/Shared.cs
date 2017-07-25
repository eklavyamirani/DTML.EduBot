using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

namespace DTML.EduBot.Constants
{
    public class Shared
    {
        public const int MaxAttempt = 2;

        public const string ClientNewLine = "<br/>";
        public const string NoMoreLessonsMessage = "That's all the lessons for today. Have a nice rest of your day!";
        public const string NoMoreQuizesMessage = "That's all the quiz questions for today.";

        public const string Yes = "Yes";
        public const string No = "No";

        public const string ChatWithBot = "Chat With Bot";
        public const string StartTheLessonPlan = "Start English Lesson Plan";

        public const string TooManyAttemptMessage = "Sorry, you have attempted too many times :(";
    }
}