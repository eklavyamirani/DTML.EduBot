namespace DTML.EduBot.LessonPlan
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Newtonsoft.Json;

    [Serializable]
    public class Topic
    {
        private ICollection<string> answerOptions = new List<string>();
        private ICollection<string> wrapUpPhrases = new List<string>();

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

        [JsonProperty("correct_answer_bot_response", Required = Required.Always)]
        public string CorrectAnswerBotResponse { get; set; }

        [JsonProperty("wrong_answer_bot_response", Required = Required.Always)]
        public string WrongAnswerBotResponse { get; set; }

        [JsonProperty("pronounciation_phrase", Required = Required.Always)]
        public string PronounciationPhrase { get; set; }


        [JsonProperty("wrap_up_phrases", Required = Required.Always)]
        public ICollection<string> WrapUpPhrases
        {
            get { return this.wrapUpPhrases; }
        }
    }
}