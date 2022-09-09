using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds
{
    public class TweetQueue
    {
        public ConcurrentQueue<TweetDetail> Tweets{ get; set; }

       public int TweetCount { get { return this.Tweets.Count; } }

        public TweetQueue()
        {
            this.Tweets = new ConcurrentQueue<TweetDetail>();
        }
    }
}
