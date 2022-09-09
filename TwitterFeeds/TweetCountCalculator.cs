using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds
{
    class TweetCountCalculator : ITweetStatCalculator
    {
        public string Calculate(List<TweetDetail> tweetsForProcessing)
        {
            return tweetsForProcessing.Count.ToString();
        }

        public string DisplayString()
        {
            return "Number of Tweets: ";
        }
    }
}
