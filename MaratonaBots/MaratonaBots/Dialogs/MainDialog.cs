using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MaratonaBots.Model;
using MaratonaBots.Service;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace MaratonaBots.Dialogs
{
    [Serializable]
    internal sealed class MainDialog : LuisDialog<object>
    {
        private const string NOT_FOUND = "Nao achei a resposta";
        private const string REI = "Rei x -";
        [NonSerialized]
        private QnaMakerClient _qnaMakerClient;

        private QnaMakerClient QnaMakerClient
        {
            get
            {
                if (_qnaMakerClient == null)
                    _qnaMakerClient = new QnaMakerClient();
                return _qnaMakerClient;
            }
        }
        public MainDialog(ILuisService model, QnaMakerClient qnaMakerClient) : base(model)
        {
            _qnaMakerClient = qnaMakerClient ??
                throw new ArgumentNullException(nameof(qnaMakerClient));
        }

        [LuisIntent("Saudacao")]
        public async Task SaudacaoIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Olá, meu nome é KingBot, eu sou o bot da Hamburgueria Seven Kings, em que posso ajudar?");
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await ObtemDadosQnAMaker(context, result);
        }

        [LuisIntent("Informacoes")]
        public async Task InformacoesIntent(IDialogContext context, LuisResult result)
        {
            await ObtemDadosQnAMaker(context, result);
        }

        private async Task ObtemDadosQnAMaker(IDialogContext context, LuisResult result)
        {
            string query = result.Query;
            Debug.WriteLine($"query={query}");

            QnaMakerResultsRoot qnaMakerResultsRoot = await QnaMakerClient.FindAnswersAsync(query);

            string summaryText = NOT_FOUND;
            if (qnaMakerResultsRoot.Answers.Count > 0)
            {
                foreach (QnaMakerResult answer in qnaMakerResultsRoot.Answers)
                {
                    Debug.WriteLine($"answer={answer.Answer} score={answer.Score}");
                }

                QnaMakerResult bestAnswer = qnaMakerResultsRoot.Answers[0];
                summaryText = bestAnswer.Answer;
                if (bestAnswer.Answer.Contains(";"))
                {
                    summaryText = string.Empty;
                    Activity resposta = (context.Activity as Activity).CreateReply();
                    resposta.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resposta.Attachments = new List<Attachment>();
                    using (var reader = new StringReader(bestAnswer.Answer))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] infos = line.Split(';');
                            if (infos.Length < 2)
                                continue;
                            var card = new HeroCard
                            {
                                Title = infos[0].Substring(REI.Length),
                                Subtitle = infos[0].Substring(0, REI.Length - 1),
                                Images = new List<CardImage> {
                                new CardImage (infos[1], infos[0].Substring (REI.Length))
                                }
                            };
                            resposta.Attachments.Add(card.ToAttachment());
                        }
                    }

                    await context.PostAsync(resposta);
                }
            }
            if (!string.IsNullOrEmpty(summaryText))
                await context.PostAsync(summaryText);
        }

        [LuisIntent("Acoes")]
        public async Task AcoesIntent(IDialogContext context, LuisResult result)
        {
            string query = result.Query;
            Debug.WriteLine($"query={query}");
            EntityRecommendation entity = result.Entities?.FirstOrDefault();
            if (entity == null || !entity.StartIndex.HasValue)
            {
                await context.PostAsync("Para quantas pessoas?");
                context.Wait(QuantidadeReceivedAsync);
                return;
            }

            int.TryParse(entity.Entity, out int quantidadePessoas);
            await FazReserva(context, quantidadePessoas);
        }

        private async Task QuantidadeReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            int.TryParse(activity.Text, out int quantidadePessoas);
            await FazReserva(context, quantidadePessoas);
        }

        private static async Task FazReserva(IDialogContext context, int quantidadePessoas)
        {
            await context.PostAsync($"OK, reservando mesa para{quantidadePessoas}");
            await context.PostAsync("Por favor enviar uma foto sua, para que possomos reconhe-lo na hora de pegar measa");
            context.Wait(ReceivePhotodAsync);
        }

        private static async Task ReceivePhotodAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity activity = await result;
            string url = activity.Attachments?.Any() == true ? activity.Attachments[0].ContentUrl : activity.Text;

            var client = new VisionComputionClient();
            bool resultRosto = await client.RostoValidoAsync(url);
            if (resultRosto)
            {
                await context.PostAsync("Reserva confirmada");
            }
            else
            {
                await context.PostAsync("Essa imagem é inválida, por favor enviar outra");
                context.Wait(ReceivePhotodAsync);
            }
        }
    }
}