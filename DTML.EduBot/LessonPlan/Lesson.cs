using Newtonsoft.Json;

namespace DTML.EduBot.LessonPlan
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Lesson
    {
        private ICollection<Topic> topics = new List<Topic>();

        [JsonProperty("lesson_title", Required = Required.Always)]
        public string LessonTitle { get; set; }

        [JsonProperty("api_url", Required = Required.Always)]
        public string APIUrl { get; set; }

        [JsonProperty("topics", Required = Required.Always)]
        public ICollection<Topic> Topics
        {
            get { return topics; }
        }

        [JsonProperty("quiz", Required = Required.Always)]
        public Quiz Quiz { get; set; }

        [JsonIgnore]
        public int currentTopic { get; set; }
    }
}