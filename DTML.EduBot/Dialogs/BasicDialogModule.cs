namespace DTML.EduBot.Dialogs
{
    using Autofac;

    public class BasicDialogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RootDialog>();
            builder.RegisterType<LessonPlanDialog>();
            builder.RegisterType<ChitChatDialog>();
            builder.RegisterType<LevelDialog>();
            builder.RegisterType<LearnEnglishDialog>();
            builder.RegisterType<NavigateDialog>();

        }
    }
}