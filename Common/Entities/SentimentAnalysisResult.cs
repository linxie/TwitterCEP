using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamInsight.Demos.Twitter.Common
{
    public class SentimentAnalysisResult
    {
        public SentimentScore Mood { get; set; }
        //public string SentimentScore { get; set; }
        public double Probability { get; set; }
    }
}
