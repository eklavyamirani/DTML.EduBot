using DTML.EduBot.UserData;

namespace DTML.EduBot
{
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using DTML.EduBot.Dialogs;
    using Scorables;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Dialogs;
    using App_Start;

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static ILifetimeScope FindContainer()
        {
            return Conversation.Container;
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Conversation.UpdateContainer(containerBuilder =>
            {
                containerBuilder.RegisterModule(new ReflectionSurrogateModule());
                containerBuilder.RegisterModule(new LessonPlanModule());
                containerBuilder.RegisterModule(new BasicDialogModule());
                containerBuilder.RegisterModule(new UserDataModule());
                containerBuilder.RegisterModule(new GlobalMessageHandlersModule());
                containerBuilder.RegisterModule(new ReflectionSurrogateModule());
                containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            });

                var config = GlobalConfiguration.Configuration;
                config.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);
        }
    }
}
