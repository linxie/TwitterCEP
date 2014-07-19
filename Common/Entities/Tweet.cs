using System;

namespace StreamInsight.Demos.Twitter.Common
{
    public class Tweet
    {
        public Int64 ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Text { get; set; }
        public string Language { get; set; }
        public string Topic { get; set; }
        public int SentimentScore { get; set; }

        public string RawJson { get; set; }
    }

}
