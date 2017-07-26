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
            else if (studentResponse.Result is string)
            {
                return new StudentResponse(studentResponse.Result);
            }

            StudentResponse answer;

            try
            {
                string result = JsonConvert.SerializeObject(studentResponse.Result);
                answer = JsonConvert.DeserializeObject<StudentResponse>(result);
            }
            catch (Exception)
            {
                // swallow exception in case of invalid Json, instead set default answer
                answer = new StudentResponse(string.Empty);
            }

            return answer;
        }

        public StudentResponse(string answer)
        {
            this.Answer = answer;
        }
    }
}