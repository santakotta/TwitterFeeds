using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using TwitterFeeds.Utils;

namespace TwitterFeeds
{
    public class HashtagRankCalculator : ITweetStatCalculator
    {
        //Regex to match the hashtags in tweet text.
        private static readonly string _pattern = @"#[a-z0-9_]+";
        /// <summary>
        /// Finds the top 10 hashtags and sends them as a string separated by comma.
        /// </summary>
        /// <param name="tweetsForProcessing">
        /// List of TweetDetail objects dequeued for the duration specified.
        /// </param>
        /// <returns></returns>
        public string Calculate(List<TweetDetail> tweetsForProcessing)
        {
            Dictionary<string, int> hashtags = new Dictionary<string, int>();
            tweetsForProcessing.ForEach(t =>
            {
                MatchCollection matches = Regex.Matches(t.Text, _pattern);
                //Add the hashtags to collection
                foreach (Match match in matches)
                {
                    if (hashtags.ContainsKey(match.Value))
                    {
                        hashtags[match.Value]++;
                    }
                    else
                    {
                        hashtags.Add(match.Value, 1);
                    }
                }
            });
            var sortedDict = (from entry in hashtags orderby entry.Value descending select entry)
                     .Take(10)
                     .ToDictionary(pair => pair.Key, pair => pair.Value);
            return String.Join(",", sortedDict.Keys);
        }

        public string DisplayString()
        {
            return "Top Hashtags: ";
        }
    }
}
