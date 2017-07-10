namespace DTML.EduBot.Qna
{
    using System;

    [Serializable]
    public class QnaRequest : IQnaRequest
    {
        public QnaRequest(string question)
        {
            Question = question;
        }

        public string Question { get; set; }
    }
}