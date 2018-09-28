using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Models
{
    public class BotCourseModel
    {
        public int AssignmentId { get; set; }
        public string CustomParameters { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string APIUrl { get; set; }
    }
}