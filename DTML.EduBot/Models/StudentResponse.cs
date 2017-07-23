using Newtonsoft.Json;

namespace DTML.EduBot.Models
{
    using System;

    public class StudentResponse
    {
        [JsonProperty("answer", Required = Required.Always)]
        public string Answer { get; set; }

        public static StudentResponse FromDynamic(dynamic studentResponse)
        {
            if (studentResponse == null)
            {
                throw new ArgumentNullException(nameof(studentResponse));
            }
            
            string result = JsonConvert.SerializeObject(studentResponse.Result);
            StudentResponse answer = JsonConvert.DeserializeObject<StudentResponse>(result);
            return answer;
        }

        public StudentResponse(string answer)
        {
            this.Answer = answer;
        }
    }
}