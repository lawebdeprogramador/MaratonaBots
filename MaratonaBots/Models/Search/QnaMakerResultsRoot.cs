using Newtonsoft.Json;
using System.Collections.Generic;

namespace MaratonaBots.Models.Search
{
    public class QnaMakerResultsRoot
    {
        [JsonProperty("answers")]
        public List<QnaMakerResult> Answers { get; set; }
    }
}