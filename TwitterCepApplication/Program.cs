using System;
using System.Configuration;
using System.Reactive.Subjects;
using System.ServiceModel;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using Microsoft.ComplexEventProcessing.ManagementService;
using StreamInsight.Demos.Twitter.Common;


namespace StreamInsight.Demos.Twitter.Client
{
    class Program
    {
        //static string _TOTAL = "Total";
        //static string _BYTOPIC = "ByTopic";
        //static string _TWEET = "Tweet";
        static int _WINDOW_LENGTH = 5;


        static void Main(string[] args)
        {
            string instanceName = ConfigurationManager.AppSettings["instance_name"];
            string appName = "bigdatademo";

            // CREATE new SERVER - this is only good for when running in embedded mode.
            Server cepServer = Server.Create(instanceName);

            // EXPOSE ENDPOINT so other clients and Event Flow Debugger can connect to it
            //RegisterManagementService(cepServer, "http://localhost:8080/MyStreamInsightServer");

            // CREATE new APPLICATION.  If one with the same name already exists, then delete it first before creating
            if (cepServer.Applications.ContainsKey(appName))
                cepServer.Applications[appName].Delete();
            Application cepApp = cepServer.CreateApplication(appName);

            // DEFINE new SOURCE, which is the Twitter Streaming API source
            TwitterConfig twitterConfig = ReadTwitterInputConfig();
            var twitterInput = cepApp.DefineStreamable<Tweet>(typeof(TwitterFactory), twitterConfig, EventShape.Point, null);


            // COMPOSE a QUERY on the Twitter source we just created.  
            var byTweetQuery = from tweet in twitterInput
                               select tweet;

            // COMPOSE a QUERY to get # of tweets by topic
            var byTopicQuery = from tweet in twitterInput
                               group tweet by tweet.Topic into groups
                               from win in groups.TumblingWindow(TimeSpan.FromSeconds(_WINDOW_LENGTH))
                               select new TweetByTopic
                               {
                                   Topic = groups.Key,
                                   TweetCount = win.Count(),
                                   AvgSentiment = win.Avg(t => t.SentimentScore),
                               };

            var byTotalQuery = from win in twitterInput.TumblingWindow(TimeSpan.FromSeconds(_WINDOW_LENGTH))
                               select new TweetSummary
                               {

                                   TweetCount = win.Count(),
                                   AvgSentiment = win.Avg(t => t.SentimentScore),
                               };

            // DEFINE Azure DB SINK 
            var azureDbSink = cepApp.DefineStreamableSink<Tweet>(typeof(AzureDbFactory),
                  GetAzureDbConfig(), EventShape.Point, StreamEventOrder.ChainOrdered);

            // DEFINE Azure Blob Storage SINK 
            var azureBlobSink = cepApp.DefineStreamableSink<Tweet>(typeof(AzureBlobFactory),
                  GetAzureBlobStorageConfig(), EventShape.Point, StreamEventOrder.ChainOrdered);

            // DEFINE Web Socket SINK to real time dashboard for sending tweet details
            var byTweetDashboardSink = cepApp.DefineStreamableSink<Tweet>(typeof(WebSocketFactory),
                  GetWebSocketConfig(QueryType.ByTweet), EventShape.Point, StreamEventOrder.ChainOrdered);

            // DEFINE Web Socket SINK to real time dashboard for sending aggregate data by topic
            var byTopicDashboardSink = cepApp.DefineStreamableSink<TweetByTopic>(typeof(WebSocketFactory),
                  GetWebSocketConfig(QueryType.ByTopic), EventShape.Point, StreamEventOrder.ChainOrdered);

            //DEFINE Web Socket SINK to real time dashboard for sending total aggregate data
            var byTotalDashboardSink = cepApp.DefineStreamableSink<TweetSummary>(typeof(WebSocketFactory),
                  GetWebSocketConfig(QueryType.ByTotal), EventShape.Point, StreamEventOrder.ChainOrdered);



            // BIND the SOURCE to the SINKS. The start the byTweetQuery
            using (
                byTweetQuery.Bind(azureDbSink)
                .With(byTweetQuery.Bind(azureBlobSink))
                .With(byTweetQuery.Bind(byTweetDashboardSink))
                .With(byTopicQuery.Bind(byTopicDashboardSink))
                .With(byTotalQuery.Bind(byTotalDashboardSink))
                .Run()
            )
            //using (byTotalQuery.Bind(byTopicDashBoardSink).With(byTweetQuery.Bind(byTotalDashboardSink)).Run())
            //using (byTotalQuery.Bind(byTopicDashBoardSink).Run())
            {
                // Wait for the user to stop the program
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("Client is running, press Enter to exit the client");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine(" ");
                Console.ReadLine();
            }


        }




        /// <summary>
        /// read settings from configuration file
        /// </summary>
        /// <returns></returns>
        private static TwitterConfig ReadTwitterInputConfig()
        {
            string probValueString = ConfigurationManager.AppSettings["sentiment_prob_threshold"];
            string apiKeys = ConfigurationManager.AppSettings["sentiment_api_key"];

            double probValue;

            if (!double.TryParse(probValueString, out probValue))
            {
                probValue = 0.6;
            }

            return new TwitterConfig()
            {
                StreamingURL = ConfigurationManager.AppSettings["twitter_streaming_url"],
                Parameters = ConfigurationManager.AppSettings["twitter_keywords"],
                Username = ConfigurationManager.AppSettings["twitter_username"],
                Password = ConfigurationManager.AppSettings["twitter_password"],
                SentimentJsonURL = ConfigurationManager.AppSettings["sentiment_json"],
                SentimentQuotaURL = ConfigurationManager.AppSettings["sentiment_quota"],
                SentimentApiKey = string.IsNullOrEmpty(apiKeys) ? new string[] { } : ConfigurationManager.AppSettings["sentiment_api_key"].Split(';'),
                SentimentProbThreshold = probValue,
            };
        }

        /// <summary>
        /// read settings from configuration file
        /// </summary>
        /// <returns></returns>       
        private static WebSocketConfig GetWebSocketConfig(QueryType type)
        {
            var config = new WebSocketConfig()
            {
                Query = type,
                URL = ConfigurationManager.AppSettings["ws_url"],
            };
            switch (type)
            {
                case QueryType.ByTotal:
                    config.SocketPort = int.Parse(ConfigurationManager.AppSettings["ws_port_total"]); break;
                case QueryType.ByTopic:
                    config.SocketPort = int.Parse(ConfigurationManager.AppSettings["ws_port_topic"]); break;
                case QueryType.ByTweet:
                    config.SocketPort = int.Parse(ConfigurationManager.AppSettings["ws_port_tweet"]); break;
            }
            return config;
        }

        private static AzureBlobCofig GetAzureBlobStorageConfig()
        {
            return new AzureBlobCofig()
            {
                StorageAccount = ConfigurationManager.AppSettings["blob_storage_account"],
                ContainerName = ConfigurationManager.AppSettings["blob_container_name"],
            };
        }

        private static AzureDbConfig GetAzureDbConfig()
        {
            return new AzureDbConfig()
            {
                ServerName = ConfigurationManager.AppSettings["db_server_name"],
                DatabaseName = ConfigurationManager.AppSettings["db_name"],
                UserName = ConfigurationManager.AppSettings["db_user"],
                Password = ConfigurationManager.AppSettings["db_user_pwd"],
            };
        }

        //    private static FbConfig readFbConfig()
        //    {
        //     AccessToken 
        //     UsernameOrUniqueId 
        //     Timeout  
        //     RefreshPeriod 
        //        return new readFbConfig();


        //}
    }
}
