using Autofac;
using MaratonaBots.Dialogs;
using MaratonaBots.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using System.Configuration;

namespace MaratonaBots.Modules
{
    internal sealed class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            string luisSubscriptionKey = ConfigurationManager.AppSettings["LuisSubscriptionKey"];
            string luisModelId = ConfigurationManager.AppSettings["LuisModelId"];

            builder.Register(x => new LuisModelAttribute(
                    luisModelId,             // modelID
                    luisSubscriptionKey,     // subscriptionKey
                    LuisApiVersion.V2
                )).AsSelf().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MainDialog>().As<IDialog<object>>().InstancePerDependency();

            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<QnaMakerService>().Keyed<ISearchService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
        }
    }
}