namespace DTML.EduBot.Qna
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class QnaResult : IQnaResult
    {
        public string Answer { get; set; }
        public IReadOnlyCollection<string> Questions { get; set; }
        public double Score { get; set; }
    }
}