using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamInsight.Demos.Twitter.Common
{
    public enum QueryType
    {
        ByTotal = 1,
        ByTopic = 2,
        ByTweet = 3
    }
    public class WebSocketConfig
    {
        public string URL { get; set; }
        public int SocketPort { get; set; }
        public QueryType Query { get; set; }
    }
}
