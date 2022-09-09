using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.Json;
using TwitterFeeds;

namespace TwitterFeeds.Test
{
    public class TweetProcessorTest
    {

        private List<TweetDetail> GetTestTweetData()
        {
            Random rand = new Random();
            TwitterFeeds.TweetDetail detail = new TwitterFeeds.TweetDetail();
            List<Tweet> tweetList = new List<Tweet>();
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string jsonString = File.ReadAllText("TestTweetData.json");
            tweetList = JsonSerializer.Deserialize<List<Tweet>>(jsonString)!;
            List<TweetDetail> testTweets = new List<TweetDetail>();
            foreach(Tweet tweet in tweetList)
            {
                testTweets.Add(tweet.Detail);
            }
            return testTweets ;
        }
        private TweetQueue GetTestTweetDataAsQueue()
        {
            List<TweetDetail> tweetList = new List<TweetDetail>();
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string jsonString = File.ReadAllText(@"..\..\..\TestTweetData.json");
            tweetList = JsonSerializer.Deserialize<List<TweetDetail>>(jsonString)!;
            TweetQueue tweetQueue = new TweetQueue();
            ConcurrentQueue<TweetDetail> testTweets = new ConcurrentQueue<TweetDetail>();
            int duration = Convert.ToInt32(ConfigurationManager.AppSettings["Duration"]);
            foreach (TweetDetail tweetDetail in tweetList)
            {
                tweetDetail.EnqueueTimeStamp = DateTime.Now.AddMilliseconds(0 - duration);
                testTweets.Enqueue(tweetDetail);
            }
            tweetQueue.Tweets = testTweets;
            return tweetQueue;
        }
        [Test]
        public void InitializeAndStartTest()
        {
            TweetQueue tweetQueue = GetTestTweetDataAsQueue();
            TweetProcessor processor = new TweetProcessor(tweetQueue);
            bool result = processor.InitializeAndStart();
            Assert.IsTrue(result);
        }

        [Test]
        public void ProcessAndPrintReportTest()
        {
            TweetQueue dummyQueue = new TweetQueue();
            TweetProcessor processor = new TweetProcessor(dummyQueue);
            List<TweetDetail> tweetsForProcessing = GetTestTweetData();
            DateTime cutoff = DateTime.Now;
            bool result = processor.ProcessAndPrintReport( tweetsForProcessing,  cutoff);
            Assert.IsTrue(result);
        }


    }
}
