using Autofac;
using Autofac.Integration.WebApi;
using MaratonaBots.Modules;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace MaratonaBots
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new DialogModule());
            builder.RegisterModule(new MainModule());

            HttpConfiguration config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
