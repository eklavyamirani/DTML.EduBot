using System.Collections.Generic;

namespace DTML.EduBot.Qna
{
    public interface IQnaResult
    {
        string Answer { get; set; }
        IReadOnlyCollection<string> Questions { get; set; }
        double Score { get; set; }
    }
}