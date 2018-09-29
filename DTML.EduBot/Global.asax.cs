using DTML.EduBot.UserData;

namespace DTML.EduBot
{
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using DTML.EduBot.Dialogs;
    using System;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using System.Configuration;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using DTML.EduBot.Common;
    using DTML.EduBot.Common.Interfaces;
    using Microsoft.Bot.Builder.History;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Error()
        {
            var e = Server.GetLastError();
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            builder.RegisterModule(new LessonPlanModule());
            builder.RegisterModule(new BasicDialogModule());

            builder
            .RegisterType<AzureTableLogger>()
            .Keyed<ILogger>(FiberModule.Key_DoNotSerialize)
            .AsSelf()
            .As<ILogger>()
            .SingleInstance();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

            var store = new TableBotDataStore(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var cache = new CachingBotDataStore(store,
                     CachingBotDataStoreConsistencyPolicy
                     .ETagBasedConsistency);

            MicrosoftAppCredentials.TrustServiceUrl("directline.botframework.com");

            builder.RegisterType<AzureActivityLogger>().As<IActivityLogger>().InstancePerDependency();

            Conversation.UpdateContainer(
                 coversation =>
                 {
                     coversation.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

                     coversation.Register(c => cache)
                     .As<IBotDataStore<BotData>>()
                     .AsSelf()
                     .InstancePerLifetimeScope();
                 });



            var config = GlobalConfiguration.Configuration;

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Server.MapPath("translation.json"));
        }
    }
}
