using DTML.EduBot.Dialogs;
using DTML.EduBot.LessonPlan;

namespace DTML.EduBot
{
    using Autofac;

    public class LessonPlanModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new LessonPlanDialog()).AsSelf().SingleInstance();
        }

        public static LessonPlan.LessonPlan LessonPlan()
        {
            // TODO: populate the lesson plan from the lesson plan metadata file, instead of hard-code in the source code
            LessonPlan.LessonPlan lessonPlan = new LessonPlan.LessonPlan();

            Lesson lessonNoun = new Lesson();
            lessonNoun.LessonName = "noun";
            Lesson lessonVerb = new Lesson();
            lessonVerb.LessonName = "verb";

            lessonPlan.Lessons.Add(lessonNoun);
            lessonPlan.Lessons.Add(lessonVerb);

            Topic topicApple = new Topic();
            topicApple.TopicName = "apple";
            Topic topicFarmer = new Topic();
            topicFarmer.TopicName = "farmer";
            Topic topicJuice = new Topic();
            topicJuice.TopicName = "juice";

            lessonNoun.Topics.Add(topicApple);
            lessonNoun.Topics.Add(topicFarmer);
            lessonNoun.Topics.Add(topicFarmer);

            Topic topicTouch = new Topic();
            topicTouch.TopicName = "touch";
            Topic topicEat = new Topic();
            topicEat.TopicName = "eat";
            Topic topicDance = new Topic();
            topicDance.TopicName = "dance";

            lessonVerb.Topics.Add(topicTouch);
            lessonVerb.Topics.Add(topicEat);
            lessonVerb.Topics.Add(topicDance);

            return lessonPlan;
        }
    }
}