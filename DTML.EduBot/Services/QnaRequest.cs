using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Services
{
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