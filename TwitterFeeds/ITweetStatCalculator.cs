using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds
{
    public interface ITweetStatCalculator
    {
        public string Calculate(List<TweetDetail> tweetsForProcessing);
        public string DisplayString();
    }
}
