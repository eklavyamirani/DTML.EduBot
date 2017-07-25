using Newtonsoft.Json;

namespace DTML.EduBot.LessonPlan
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Lesson
    {
        private ICollection<Topic> topics = new List<Topic>();
        private ICollection<TestQuestion> test = new List<TestQuestion>();

        [JsonProperty("lesson_title", Required = Required.Always)]
        public string LessonTitle { get; set; }

        [JsonProperty("topics", Required = Required.Always)]
        public ICollection<Topic> Topics
        {
            get { return topics; }
        }

        [JsonProperty("test", Required = Required.Always)]
        public ICollection<TestQuestion> Test
        {
            get { return test; }
        }

        [JsonIgnore]
        public int currentTopic { get; set; }
    }
}