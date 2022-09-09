using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterFeeds
{
    public class TweetReader
    {
        private HttpClient _client;
        private static String Uri = ConfigurationManager.AppSettings["apiUrl"];
        private static String Token = ConfigurationManager.AppSettings["token"];
        private static Encoding StreamEncoding = Encoding.GetEncoding("UTF-8");
        private TweetQueue _tweetQueue;
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public Queue TweetQueue { get; private set; }
        public TweetReader(TweetQueue tweetQueue)
        {
            this._tweetQueue = tweetQueue;
        }
        public async void StartReadingTweets()
        {
            try
            {
                CreateHttpClient();
                StreamReader reader = await GetRequestStream();
                QueueTweets(reader);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
        }
        /// <summary>
        /// Creates new HttpClient in case of disconnections.
        /// </summary>
        /// <returns></returns>
        private void CreateHttpClient()
        {
            this._client = new HttpClient();
            this._client.DefaultRequestHeaders.Clear();
            this._client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
        }

        private async Task<StreamReader> GetRequestStream()
        {
            StreamReader reader = null;
            try
            {
                var response = await _client.GetStreamAsync(Uri);
                reader = new StreamReader(response, StreamEncoding);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return reader;
        }

        private void QueueTweets(StreamReader reader)
        {
            while (true)
            {
                String tweetStr = reader.ReadLine();

                if (!String.IsNullOrEmpty(tweetStr))
                {
                    var tweetData = JsonSerializer.Deserialize<Tweet>(tweetStr, _options);
                    //Check if errors in the response.
                    if (HasErrors(tweetData))
                    {
                        HandleErrors(tweetData);
                    }
                    else
                    {
                        tweetData.Detail.EnqueueTimeStamp = DateTime.Now;
                        tweetData.Detail.IsProcessed = false;
                        _tweetQueue.Tweets.Enqueue(tweetData.Detail);
                    }
                }
            
            }
        }

        private bool HasErrors(Tweet tweetData)
        {
            return tweetData.Errors != null && tweetData.Errors.Count > 0;
        }
        private void HandleErrors(Tweet tweetData)
        {
           //Handle error
            tweetData.Errors.ForEach(e =>
            {
                //Construct error message and write to log.
                Console.WriteLine(String.Format("Title: {0}, Type: {1}, Detail: {2}, DisconnectType: {3}", e.Title, e.Type, e.Detail, e.DisconnectType));
                //Handle throttling for 429 error.
                switch (e.Title)
                {
                    case "operational-disconnect":
                        {
                            //Handle Upstream operational disconnect
                            break;
                        }
                    case "ConnectionException":
                        {
                            //Handle too many connections issue.
                            //Check for headers:
                            //1.X-rate-limit-limit
                            //2.X-rate-limit-remaining
                            //3.X-rate-limit-reset
                            break;
                        }
                    default:
                        break;
                }
            });
            
        }
    }
}
