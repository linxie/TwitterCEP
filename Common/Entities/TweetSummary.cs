using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamInsight.Demos.Twitter.Common
{
    public class TweetSummary
    {
        //public double Timestamp { get; set; }
        //public string Subject { get; set; }
        public long TweetCount { get; set; }
        public double AvgSentiment { get; set; }
    }
}
