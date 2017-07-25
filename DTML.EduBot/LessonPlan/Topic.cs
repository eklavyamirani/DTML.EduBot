namespace DTML.EduBot.LessonPlan
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;
    using Newtonsoft.Json;

    [Serializable]
    public class Topic
    {
        private ICollection<string> answerOptions = new List<string>();

        [JsonProperty("question", Required = Required.Always)]
        public string Question { get; set; }

        [JsonProperty("image_url", Required = Required.Always)]
        public string ImageUrl { get; set; }

        [JsonProperty("answer_options", Required = Required.Always)]
        public ICollection<string> AnswerOptions
        {
            get { return this.answerOptions; }
        }

        [JsonProperty("correct_answer", Required = Required.Always)]
        public string CorrectAnswer { get; set; }

        [DefaultValue("Correct! Now, can you type the word?")]
        [JsonProperty("correct_answer_bot_response", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CorrectAnswerBotResponse { get; set; }

        [DefaultValue("Sorry, incorrect, try again")]
        [JsonProperty("wrong_answer_bot_response", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string WrongAnswerBotResponse { get; set; }

        [DefaultValue("Good work! Here is how you say it, Repeat after me.")]
        [JsonProperty("pronounciation_phrase", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string PronounciationPhrase { get; set; }
    }
}