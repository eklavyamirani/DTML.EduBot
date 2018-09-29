using System.IO;
using System.Web;
using DTML.EduBot.Dialogs;
using DTML.EduBot.LessonPlan;
using Newtonsoft.Json;
using System.Configuration;

namespace DTML.EduBot
{
    using Autofac;
    using DTML.EduBot.Models;
    using DTML.EduBot.Utilities;
    using System.Collections.Generic;

    public class LessonPlanModule : Module
    {
        public static LessonPlan.LessonPlan LessonPlan = new LessonPlan.LessonPlan();

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            LessonPlan = LoadLessonPlan();

        }

        private LessonPlan.LessonPlan LoadLessonPlan()
        {
            string lessonPlanAPI = ConfigurationManager.AppSettings["LessonServiceAPI"];
            LessonPlan.LessonPlan lessonPlan = new LessonPlan.LessonPlan();
            List<BotCourseModel> lessons = LessonPlanHelper.GetLessonPlanAsync<List<BotCourseModel>>(lessonPlanAPI).Result;

            foreach (BotCourseModel lesson in lessons)
            {
                lessonPlan.Lessons.Add(new Lesson() { LessonTitle = lesson.Name, currentTopic = lesson.AssignmentId, APIUrl = lesson.APIUrl });
            }

            return lessonPlan;
        }
    }
}