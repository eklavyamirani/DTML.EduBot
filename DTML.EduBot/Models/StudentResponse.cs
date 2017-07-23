namespace DTML.EduBot.Models
{
    using System;

    public class StudentResponse
    {
        public string Answer { get; set; }

        public static StudentResponse FromDynamic(dynamic studentResponse)
        {
            if (studentResponse == null)
            {
                throw new ArgumentNullException(nameof(studentResponse));
            }

            var answer = studentResponse.Answer as string;
            return new StudentResponse(answer);
        }

        public StudentResponse(string answer)
        {
            this.Answer = answer;
        }
    }
}