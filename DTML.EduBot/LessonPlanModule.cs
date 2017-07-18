using System.IO;
using System.Web;
using DTML.EduBot.Dialogs;
using DTML.EduBot.LessonPlan;
using Newtonsoft.Json;

namespace DTML.EduBot
{
    using Autofac;

    public class LessonPlanModule : Module
    {
        public static LessonPlan.LessonPlan LessonPlan = new LessonPlan.LessonPlan();

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            LessonPlan = LoadLessonPlan();

            builder.RegisterInstance(new LessonPlanDialog()).AsSelf().SingleInstance();
        }

        private LessonPlan.LessonPlan LoadLessonPlan()
        {
            LessonPlan.LessonPlan lessonPlan = new LessonPlan.LessonPlan();

            string lessonPlanPath = HttpContext.Current.Server.MapPath("~/LessonPlan/lesson_plan.json");

            using (StreamReader reader = new StreamReader(lessonPlanPath))
            {
                string lessonPlanJson = reader.ReadToEnd();
                lessonPlan = JsonConvert.DeserializeObject<LessonPlan.LessonPlan>(lessonPlanJson);
            }

            return lessonPlan;
        }
    }
}