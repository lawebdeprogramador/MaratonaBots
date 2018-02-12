using MaratonaBots.Dialogs;
using MaratonaBots.Service;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Configuration;

namespace MaratonaBots.Factory
{
    internal static class DialogFactory
    {
        public static IDialog<object> CreateDialog()
        {
            return new MainDialog(new LuisService(new LuisModelAttribute(
                    modelID: ConfigurationManager.AppSettings["LuisModelId"],
                    subscriptionKey: ConfigurationManager.AppSettings["LuisSubscriptionKey"]
                )), new QnaMakerService());
        }
    }
}