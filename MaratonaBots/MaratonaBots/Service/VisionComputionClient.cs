using MaratonaBots.Model;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MaratonaBots.Service
{
    public class VisionComputionClient
    {
        public async Task<bool> RostoValidoAsync(string url)
        {
            string vcSubscriptionKey = ConfigurationManager.AppSettings["VCSubscriptionKey"];

            string locationServer = ConfigurationManager.AppSettings["VCUrl"];

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", vcSubscriptionKey);

                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["visualFeatures"] = "Faces,Adult";


                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{locationServer}?{queryString}", new { Url = url });
                var result = await response.Content.ReadAsAsync<FaceValidationResult>();

                return !(result.Adult.IsAdultContent || result.Adult.IsRacyContent)
                        && result.Faces.Count() > 1;
            }
        }
    }
}