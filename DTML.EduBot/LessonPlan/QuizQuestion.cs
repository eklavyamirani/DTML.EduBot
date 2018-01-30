using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Newtonsoft.Json;

namespace DTML.EduBot.LessonPlan
{
    [Serializable]
    public class QuizQuestion
    {
        [JsonProperty("question", Required = Required.Always)]
        public string Question { get; set; }

        [JsonProperty("image_url", Required = Required.Always)]
        public string ImageUrl { get; set; }

        [JsonProperty("correct_answers", Required = Required.Always)]
        public List<string> CorrectAnswers { get; set; }

        [DefaultValue("Correct! Now, can you type the word?")]
        [JsonProperty("correct_answer_bot_response", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CorrectAnswerBotResponse { get; set; }

        [DefaultValue("Sorry, incorrect, try again")]
        [JsonProperty("wrong_answer_bot_response", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string WrongAnswerBotResponse { get; set; }
    }
}