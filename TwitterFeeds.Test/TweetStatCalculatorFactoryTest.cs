using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds.Test
{
    
    public class TweetStatCalculatorFactoryTest
    {
        [Test]
        public void BuildStatCalculatorTest()
        {
            ITweetStatCalculator calc1 = TweetStatCalculatorFactory.BuildStatCalculator("FindTweetCount");
            Assert.IsTrue(calc1.GetType().Name == "TweetCountCalculator");
            ITweetStatCalculator calc2 = TweetStatCalculatorFactory.BuildStatCalculator("FindTopHashtags");
            Assert.IsTrue(calc2.GetType().Name == "HashtagRankCalculator");
            ITweetStatCalculator calc3 = TweetStatCalculatorFactory.BuildStatCalculator("FindTweetCount");
            Assert.IsFalse(calc3.GetType().Name == "HashtagRankCalculator");
            ITweetStatCalculator calc4 = TweetStatCalculatorFactory.BuildStatCalculator("kjdfjk");
            Assert.IsNull(calc4);
        }
    }
}
