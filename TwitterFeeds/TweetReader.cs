using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterFeeds
{/// <summary>
/// 
/// </summary>
    public class TweetReader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const bool CONTINUE_READING = true;
        
        //private HttpClient _client;
        private static readonly String ApiEndpoint = ConfigurationManager.AppSettings["apiUrl"];
        private static readonly String Token = ConfigurationManager.AppSettings["token"];
        private static Encoding StreamEncoding = Encoding.GetEncoding("UTF-8");
        private TweetQueue _tweetQueue;
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public Queue TweetQueue { get; private set; }
        /// <summary>
        /// 
        /// Takes the queue datastructure for storing tweets.
        /// </summary>
        /// <param name="tweetQueue"></param>
        public TweetReader(TweetQueue tweetQueue)
        {
            this._tweetQueue = tweetQueue;
        }
        /// <summary>
        /// 
        /// Will be used by clients to create a connect and start reading tweets and queing them for further processing.
        /// </summary>
        public void StartReadingTweets()
        {
            try
            {
                HttpClient httpClient = CreateHttpClient();
                QueueTweets(httpClient);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            
        }
        /// <summary>
        /// Creates new HttpClient for connecting with API.
        /// </summary>
        /// <returns></returns>
        //private HttpClient CreateHttpClient()
        private HttpClient CreateHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
            return httpClient;
        }
        /// <summary>
        /// 
        /// Executes the GET request and queues the serialized response.
        /// </summary>
        //public void QueueTweets(HttpClient httpClient)
        public void QueueTweets(HttpClient httpClient)
        {
            StreamReader reader = null;
            try
            {
                Uri uriObj = new Uri(ApiEndpoint);
                var response = httpClient.GetStreamAsync(ApiEndpoint).GetAwaiter().GetResult();
                reader = new StreamReader(response, StreamEncoding);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new TweetReaderException("Exception Reading tweet from api endpoint.", ex);
            }
            while (CONTINUE_READING)
            {
                String tweetStr = reader.ReadLine();
                if (!String.IsNullOrEmpty(tweetStr))
                {
                    var tweetData = JsonSerializer.Deserialize<Tweet>(tweetStr, _options);
                    //Check if errors in the JSON response.
                    if (HasErrors(tweetData))
                    {
                        HandleErrors(tweetData);
                    }
                    else
                    {
                        tweetData.Detail.EnqueueTimeStamp = DateTime.Now;
                        tweetData.Detail.IsProcessed = false;

                        _tweetQueue.Tweets.Enqueue(tweetData.Detail);
                        log.Debug("Queued::" + tweetData.Detail.Id + "Size::" + _tweetQueue.TweetCount);
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// Checks if response has Errors collection and verifies the error count is greater than zero.
        /// </summary>
        /// <param name="tweetData">
        /// </param>
        /// <returns></returns>
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
                log.Error(string.Format("Title: {0}, Type: {1}, Detail: {2}, DisconnectType: {3}", e.Title, e.Type, e.Detail, e.DisconnectType));
                //Handle throttling for 429 error.
                switch (e.Title)
                {
                    case "operational-disconnect":
                        {
                            //Handle Upstream operational disconnect
                            string errorStr = "Encountered operational-disconnect in the response.  Reconnect logic needs to be implemented according to Twitter documentation.";
                            log.Error(errorStr);
                            throw new TweetReaderException(errorStr);
                        }
                    case "ConnectionException":
                        {
                            //Handle too many connections issue.
                            string errorStr = "Encountered ConnectionException in the response.  Reconnect logic needs to be implemented according to Twitter documentation.";
                            log.Error(errorStr);
                            throw new TweetReaderException(errorStr);
                        }
                    default:
                        break;
                }
            });
            
        }
    }
}
