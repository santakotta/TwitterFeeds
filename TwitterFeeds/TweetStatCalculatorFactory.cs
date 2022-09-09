using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds
{
    /// <summary>
    /// Factory to get the calculators matching the StatCalculators key defined in the config.
    /// 
    /// </summary>
    public class TweetStatCalculatorFactory
    {
        /// <summary>
        /// Gets the calculator based on key passed as argument.
        ///
        /// </summary>
        /// <param name="calculatorType">
        /// Matches the key with appsettings key: StatCalculators.
        /// </param>
        /// <returns></returns>
        public static ITweetStatCalculator BuildStatCalculator(String calculatorType)
        {
            Dictionary<string, ITweetStatCalculator> calculatorDict = ConstructCalculatorDict();
            return calculatorDict.GetValueOrDefault(calculatorType);
        }
        /// <summary>
        /// Dictionay that maps the calculator objects to the keys defined in config.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, ITweetStatCalculator> ConstructCalculatorDict()
        {
            Dictionary<string, ITweetStatCalculator> calculatorDict = new Dictionary<string, ITweetStatCalculator>();
            calculatorDict.Add("FindTweetCount", new TweetCountCalculator());
            calculatorDict.Add("FindTopHashtags", new HashtagRankCalculator());
            return calculatorDict;
        }
    }
}
