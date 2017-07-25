using Newtonsoft.Json;

namespace DTML.EduBot.LessonPlan
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Quiz
    {
        private ICollection<QuizQuestion> questions = new List<QuizQuestion>();

        [JsonProperty("questions", Required = Required.Always)]
        public ICollection<QuizQuestion> Questions
        {
            get { return questions; }
        }

        [JsonIgnore]
        public int currentQuestion { get; set; }
    }
}