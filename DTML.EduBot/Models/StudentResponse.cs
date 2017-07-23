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

<<<<<<< HEAD
            string result = JsonConvert.SerializeObject(studentResponse.Result);
            StudentResponse answer = JsonConvert.DeserializeObject<StudentResponse>(result);
            return answer;
=======
            if (studentResponse.Answer == null)
            {
                studentResponse.Answer = string.Empty;
            }

            var answer = studentResponse.Answer.ToString();
            return new StudentResponse(answer);
>>>>>>> 7941c63b0b4cc99b6b51a1510ab84dfbbf877389
        }

        public StudentResponse(string answer)
        {
            this.Answer = answer;
        }
    }
}