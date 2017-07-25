using DTML.EduBot.UserData;

namespace DTML.EduBot
{
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using DTML.EduBot.Dialogs;

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static ILifetimeScope FindContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver) config.DependencyResolver;
            return resolver.Container;
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            builder.RegisterModule(new LessonPlanModule());
            builder.RegisterModule(new BasicDialogModule());
            builder.RegisterModule(new UserDataModule());

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var config = GlobalConfiguration.Configuration;

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
