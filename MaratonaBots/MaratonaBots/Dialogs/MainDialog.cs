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
        private readonly QnaMakerService _qnaMakerService;
        
        private QnaMakerService QnaMakerService
        {
            get
            {
                if (_qnaMakerService != null)
                    return _qnaMakerService;
                return new QnaMakerService();
            }
        }
        public MainDialog(ILuisService model, QnaMakerService qnaMakerService)
            : base(model)
        {
            _qnaMakerService = qnaMakerService ?? throw new ArgumentNullException(nameof(qnaMakerService));
        }

        [LuisIntent("Saudacao")]
        public async Task SaudacaoIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Olá, meu nome é KingBot, eu sou o bot da Hamburgueria Seven Kings, em que posso ajudar?");
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            string query = result.Query;
            Debug.WriteLine($"query={query}");

            QnaMakerResultsRoot qnaMakerResultsRoot = await QnaMakerService.FindAnswersAsync(query);

            string summaryText = NOT_FOUND;
            if (qnaMakerResultsRoot.Answers.Count > 0)
            {
                foreach (QnaMakerResult answer in qnaMakerResultsRoot.Answers)
                {
                    Debug.WriteLine($"answer={answer.Answer} score={answer.Score}");
                }
                QnaMakerResult bestAnswer = qnaMakerResultsRoot.Answers[0];
                summaryText = bestAnswer.Answer;
            }
            await context.PostAsync(summaryText);
        }

        [LuisIntent("Informacoes")]
        public async Task InformacoesIntent(IDialogContext context, LuisResult result)
        {
            string query = result.Query;
            Debug.WriteLine($"query={query}");

            QnaMakerResultsRoot qnaMakerResultsRoot = await QnaMakerService.FindAnswersAsync(query);

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
                                Images = new List<CardImage>
                                {
                                    new CardImage(infos[1], infos[0].Substring(REI.Length))
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
            await context.PostAsync("Fazendo");
        }
    }
}