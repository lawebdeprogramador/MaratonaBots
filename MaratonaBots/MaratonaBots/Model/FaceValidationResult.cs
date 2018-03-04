using System.Collections.Generic;

namespace MaratonaBots.Model
{
    public class FaceValidationResult
    {
        public IEnumerable<Faces> Faces { get; set; }

        public Adult Adult { get; set; }
    }

    public class Faces
    {
        public int Age { get; set; }
        public string Gender { get; set; }
        public IEnumerable<FaceRectangle> FaceRectangle { get; set; }
    }

    public class FaceRectangle
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Adult
    {
        public bool IsAdultContent { get; set; }
        public decimal AdultScore { get; set; }

        public bool IsRacyContent { get; set; }
        public decimal RacyScore { get; set; }
    }
}