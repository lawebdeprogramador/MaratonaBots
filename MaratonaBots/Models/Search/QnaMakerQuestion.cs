using System;
using Newtonsoft.Json;

namespace MaratonaBots.Models.Search
{
    public class QnaMakerQuestion
    {
        public QnaMakerQuestion()
        {

        }

        public QnaMakerQuestion(string question)
        {
            Question = question ?? throw new ArgumentNullException(nameof(question));
        }

        [JsonProperty("question")]
        public string Question { get; set; }
    }
}