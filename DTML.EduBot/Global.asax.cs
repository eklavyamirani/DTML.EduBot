using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;

namespace DTML.EduBot
{
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

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var config = GlobalConfiguration.Configuration;

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
