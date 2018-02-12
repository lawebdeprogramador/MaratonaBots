using System;

namespace MaratonaBots.Model
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

        
        public string Question { get; set; }
    }
}