using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Timers;
using System.Linq;
using Microsoft.Extensions.Logging;
using log4net;
using System.Reflection;

namespace TwitterFeeds
{
    /// <summary>
    /// Processes the tweets by dequing and doing required analytics.
    /// </summary>
    public class TweetProcessor
    {
        private const bool CONTINUE_TWEET_PROCESSING = true;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //used to schedule the processor for a configured duration.
        private Timer _timer;
        //Has the tweets read from API.  Will be dequed for processing.
        private TweetQueue _tweetQueue;
        //Time duration how often the processor needs to be run
        private static int Duration = Convert.ToInt32(ConfigurationManager.AppSettings["process-timer-duration"]);
        private static string _calculatorKeys = ConfigurationManager.AppSettings["StatCalculators"];
        public TweetProcessor(TweetQueue tweetQueue)
        {
            this._tweetQueue = tweetQueue;
        }

        /// <summary>
        /// Initializes the timer and starts the process flow.
        /// 
        /// </summary>
        public bool InitializeAndStart()
        {
            log.Info("In InitializeAndStart:: Queue size::" + this._tweetQueue.TweetCount);
            InitializeProcessTweets();
            return true;
            
        }
        /// <summary>
        /// Local private method to initialize the timer and add handler for processing tweets.
        /// </summary>
        private bool InitializeProcessTweets()
        {
            try
            {
                // Create a timer with set interval from config
                this._timer = new System.Timers.Timer(Duration);
                // Hook up the Elapsed event for the timer. 
                this._timer.Elapsed += new ElapsedEventHandler((sender, e) =>
                {
                    ProcessTweets(sender, e);
                });
                this._timer.Enabled = true;
                log.Info("Timer enabled.");
            }catch(Exception ex)
            {
                log.Error(ex);
                TweetStatReporter display = new TweetStatReporter(DateTime.Now);
                display.PrintErrorToConsole("Error Processing Tweets.");
                return false;
            }
            return true;
            
        }
        /// <summary>
        /// Calculate rate of tweets received per 20 seconds
        /// List top ten hashtags
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected bool ProcessTweets(Object source, ElapsedEventArgs e)
        {
            List<String> topTenHashTags = new List<string>();
            DateTime cutoff = DateTime.Now;
            log.Info(String.Format("STARTED DEQUEUE: CUT OFF: {0}", cutoff.ToString("MM /dd/yyyyThh:mm:ss:fff")));
            Dictionary<string, int> hashtagDictionary = new Dictionary<string, int>();
            SortedDictionary<int, String> rankedDictionary = new SortedDictionary<int, String>();
            List<TweetDetail> tweetsForProcessing = new List<TweetDetail>();
            try
            {
                while (CONTINUE_TWEET_PROCESSING)
                {
                    if (_tweetQueue.Tweets.Count <= 0)
                        break;
                    TweetDetail currentItem;
                    _tweetQueue.Tweets.TryPeek(out currentItem);
                    if (currentItem.EnqueueTimeStamp <= cutoff)
                    {
                        //tweetCount++;
                        _tweetQueue.Tweets.TryDequeue(out currentItem);
                        currentItem.IsProcessed = true;
                        tweetsForProcessing.Add(currentItem);
                        log.Info(String.Format("DEQUEUED:::::id: {0}, EnqueTimeStamp: {2}, IsProcessed: {3}", currentItem.Id, currentItem.Text, currentItem.EnqueueTimeStamp, currentItem.IsProcessed));
                    }
                    else
                    {
                        break;
                    }
                }
                //Process if tweets found in the given time duration
                if (tweetsForProcessing.Count > 0)
                {
                    ProcessAndPrintReport(tweetsForProcessing, cutoff);
                }
                log.Info(String.Format("Tweet Processor End at {0}", DateTime.Now.ToString("MM/dd/yyyyThh:mm:ss:fff")));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                TweetStatReporter display = new TweetStatReporter(DateTime.Now);
                display.PrintErrorToConsole("Error Processing Tweets.");
                return false;
            }
            return true;

        }
        /// <summary>
        /// 
        /// Use the calculators to calculate the analytics and display the results on console
        /// </summary>
        /// <param name="tweetsForProcessing"></param>
        /// <param name="cutoff"></param>
        public bool ProcessAndPrintReport(List<TweetDetail> tweetsForProcessing, DateTime cutoff)
        {
            ITweetStatCalculator calculator = null;
            List<ITweetStatCalculator> calculators = new List<ITweetStatCalculator>();
            try
            {
                String[] calculatorKeyArray = _calculatorKeys.Split(",");
                foreach (String key in calculatorKeyArray)
                {
                    calculator = TweetStatCalculatorFactory.BuildStatCalculator(key);
                    if (calculator != null)
                    {
                        calculators.Add(calculator);
                    }
                }
                List<string> statResultStrs = new List<string>();
                calculators.ForEach(c => statResultStrs.Add(c.Calculate(tweetsForProcessing)));
                TweetStatReporter display = new TweetStatReporter(cutoff);
                display.PrintResultToConsole(statResultStrs);
            }catch(Exception ex)
            {
                log.Error(ex);
                TweetStatReporter display = new TweetStatReporter(cutoff);
                display.PrintErrorToConsole(ex.Message);
                return false;
            }
            return true;
        }
    }
}
