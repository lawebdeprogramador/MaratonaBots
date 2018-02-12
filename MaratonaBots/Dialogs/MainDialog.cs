using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace MaratonaBots.Dialogs
{
    [Serializable]
    public class MainDialog : LuisDialog<object>
    {
        public MainDialog(ILuisService luisService)
            : base(luisService)
        {

        }

        [LuisIntent("Saudacao")]
        public async Task SaudacaoIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Olá, meu nome é KingBot, eu sou o bot da Hamburgueria Seven Kings, em que posso ajudar?");
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            Task searching = context.PostAsync("Procurando na base de conhecimento...");



            await searching;
        }

        [LuisIntent("Informacoes")]
        public async Task InformacoesIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Fazendo");
        }

        [LuisIntent("Acoes")]
        public async Task AcoesIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Fazendo");
        }
    }
}