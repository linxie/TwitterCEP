namespace StreamInsight.Demos.Twitter.Common
{
    public enum TwitterMode
    {
        Firehose,
        Sample,
        Filter
    }

    /// <summary>
    /// Configuration for textToAnalyze adapter
    /// </summary>
    public class TwitterConfig
    {
        public string StreamingURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Parameters { get; set; }
        public TwitterMode Mode { get; set; }
        public int Timeout { get; set; }
        public string SentimentJsonURL { get; set; }
        public string SentimentQuotaURL { get; set; }
        public string[] SentimentApiKey { get; set; }
        public double SentimentProbThreshold { get; set; }

        public TwitterConfig()
        {
            Mode = TwitterMode.Filter;
            Timeout = 300000;
            Parameters = "Microsoft";
            SentimentProbThreshold = 0.6;
        }
    }
}
