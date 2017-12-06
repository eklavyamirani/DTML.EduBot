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

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            builder.RegisterModule(new LessonPlanModule());
            builder.RegisterModule(new BasicDialogModule());

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var store = new TableBotDataStore(ConfigurationManager.AppSettings["StorageConnectionString"]);
            MicrosoftAppCredentials.TrustServiceUrl("directline.botframework.com ");

            Conversation.UpdateContainer(
                 coversation =>
                 {
                 coversation.Register(c => store)
                .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                .AsSelf()
                .SingleInstance();

                 coversation.Register(c => new CachingBotDataStore(store,
                 CachingBotDataStoreConsistencyPolicy
                 .ETagBasedConsistency))
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
