using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamInsight.Demos.Twitter.Common
{
    public class TweetByTopic
    {
        public string Topic { get; set; }
        public long TweetCount { get; set; }
        public double AvgSentiment { get; set; }
    }

   
}
