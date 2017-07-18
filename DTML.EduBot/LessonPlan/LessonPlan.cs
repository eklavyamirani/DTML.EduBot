using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DTML.EduBot.LessonPlan
{
    public class LessonPlan
    {
        private ICollection<Lesson> lessons = new Collection<Lesson>();

        [JsonProperty("lessons", Required = Required.Always)]
        public ICollection<Lesson> Lessons
        {
            get
            {
                return lessons;
            }
        }
    }
}