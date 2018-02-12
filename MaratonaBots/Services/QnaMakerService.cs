using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using MaratonaBots.Models.Search;

namespace MaratonaBots.Services
{
    internal sealed class QnaMakerService : ISearchService
    {
        public async Task<QnaMakerResultsRoot> FindAnswersAsync(string question)
        {
            string qnamakerSubscriptionKey = ConfigurationManager.AppSettings["QnaMakerSubscriptionKey"];
            string knowledgebaseId = ConfigurationManager.AppSettings["QnaMakerKnowledgebaseId"];

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);

                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{knowledgebaseId}/generateAnswer", new QnaMakerQuestion(question));
                return await response.Content.ReadAsAsync<QnaMakerResultsRoot>();
            }
        }
    }
}