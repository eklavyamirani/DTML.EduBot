using Autofac;

namespace DTML.EduBot.UserData
{
    public class UserDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<UserDataRepository>()
                .As<IUserDataRepository>().SingleInstance();
        }
    }
}