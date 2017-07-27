using Autofac;
using DTML.EduBot.Scorables;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.App_Start
{
    public class GlobalMessageHandlersModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(c => new HelpScorables(c.Resolve<IDialogTask>()))
                   .As<IScorable<IActivity, double>>()
                   .InstancePerLifetimeScope();
        }
    }
}