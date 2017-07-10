using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Services
{
    [Serializable]
    public class QnaResult : IQnaResult
    {
        public string Answer { get; set; }
        public IReadOnlyCollection<string> Questions { get; set; }
        public double Score { get; set; }
    }
}