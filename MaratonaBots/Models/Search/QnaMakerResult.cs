using Newtonsoft.Json;

namespace MaratonaBots.Models.Search
{
    public class QnaMakerResult
    {
        // The top answer found in the QnA Service.
        [JsonProperty("answer")]
        public string Answer { get; set; }

        // The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}