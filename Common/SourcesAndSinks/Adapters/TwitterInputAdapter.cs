using System;
using System.Globalization;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using Newtonsoft.Json.Linq;

namespace StreamInsight.Demos.Twitter.Common
{
    /// <summary>
    /// Simple input adapter that reads twits
    /// </summary>
    public class TwitterInput : TypedPointInputAdapter<Tweet>
    {
        private readonly static IFormatProvider DateFormatProvider = CultureInfo.GetCultureInfo("en-us").DateTimeFormat;
        private const string DateFormatString = "ddd MMM dd HH:mm:ss yyyy";
        private PointEvent<Tweet> pendingEvent;
        private Sentiment140 _sentimentService;
        private TwitterStreaming _twitterStreaming;
        private TwitterConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration for this adapter</param>
        public TwitterInput(TwitterConfig config)
        {
            _config = config;
            this._twitterStreaming = new TwitterStreaming(config);

            this._sentimentService = new Sentiment140();
            //this._sentimentService = new ViralheatSentiment(
            //        config.SentimentJsonURL,
            //        config.SentimentQuotaURL,
            //        config.SentimentApiKey,
            //        config.SentimentProbThreshold);
        }

        public override void Start()
        {
            ProduceEvents();
        }

        public override void Resume()
        {
            ProduceEvents();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Main loop
        /// </summary>
        private void ProduceEvents()
        {
            var currEvent = default(PointEvent<Tweet>);

            using (var streamReader = this._twitterStreaming.Read())
            {
                EnqueueCtiEvent(DateTimeOffset.Now);

                try
                {
                    // Loop until stop signal
                    while (AdapterState != AdapterState.Stopping)
                    {
                        if (pendingEvent != null)
                        {
                            currEvent = pendingEvent;
                            pendingEvent = null;
                        }
                        else
                        {
                            try
                            {
                                // Read from source
                                var line = streamReader.ReadLine();

                                if (string.IsNullOrEmpty(line)) continue;

                                // Parse
                                var jObject = JObject.Parse(line);
                                string text = Unquote(jObject.SelectToken("text").Value<string>());
                                string topic = TextAnalysis.DetermineTopc(text, _config.Parameters);
                                string language = (null == jObject.SelectToken("user.lang")) ? string.Empty : jObject.SelectToken("user.lang").Value<string>();

                                // filter out tweets we don't care about: non-english tweets or deleted tweets
                                if ((jObject["delete"] != null) ||
                                    (language != "en") ||
                                    (topic == "Unknown"))
                                    continue;

                                var tweet = new Tweet();
                                tweet.ID = jObject.SelectToken("id_str").Value<Int64>();
                                tweet.CreatedAt = ParseTwitterDateTime(jObject.SelectToken("created_at").Value<string>());
                                tweet.UserName = Unquote(jObject.SelectToken("user.screen_name").Value<string>());
                                tweet.ProfileImageUrl = jObject.SelectToken("user.profile_image_url").Value<string>();
                                tweet.Text = text;
                                tweet.Topic = topic;
                                tweet.RawJson = line;

                                this.Sentiment(tweet, text);

                                // Produce INSERT event
                                currEvent = CreateInsertEvent();
                                currEvent.StartTime = DateTimeOffset.Now;
                                currEvent.Payload = tweet;
                                pendingEvent = null;
                                //PrintEvent(currEvent);
                                Enqueue(ref currEvent);
                                
                                // Also send a CTI event
                                EnqueueCtiEvent(DateTimeOffset.Now);
                            }
                            catch (Exception ex)
                            {
                                // Error handling should go here
                                Console.WriteLine("EXCEPTION RAISED in TwitterInputAdapter: " + ex.Message);
                            }
                        }
                    }

                    if (pendingEvent != null)
                    {
                        currEvent = pendingEvent;
                        pendingEvent = null;
                    }

                    PrepareToStop(currEvent);
                    Stopped();
                }
                catch (Exception e)
                {
                    Console.WriteLine(this.GetType().ToString() + ".ProduceEvents - " + e.Message + e.StackTrace);
                }
            }
        }

        private DateTime ParseTwitterDateTime(string p)
        {
            p = p.Replace("+0000 ", "");
            DateTimeOffset result;

            if (DateTimeOffset.TryParseExact(p, DateFormatString, DateFormatProvider, DateTimeStyles.AssumeUniversal, out result))
                return result.DateTime;
            else
                return DateTime.Now;
        }

        private void PrepareToStop(PointEvent<Tweet> currEvent)
        {
            //EnqueueCtiEvent(DateTime.Now);
            if (currEvent != null)
            {
                // Do this to avoid memory leaks
                ReleaseEvent(ref currEvent);
            }
        }

        private void PrepareToResume(PointEvent<Tweet> currEvent)
        {
            pendingEvent = currEvent;
        }

        private string Unquote(string str)
        {
            return str.Trim('"');
        }

        private void Sentiment(Tweet tweet, string text)
        {
            try
            {
                var result = this._sentimentService.Analyze(text);
                tweet.SentimentScore = (int) result.Mood;
            }
            catch (Exception e)
            {
                Console.WriteLine(this.GetType().ToString() + ".Sentiment - " + e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// Debugging function
        /// </summary>
        /// <param name="evt"></param>
        private void PrintEvent(PointEvent<Tweet> evt)
        {
        }
    }
}
