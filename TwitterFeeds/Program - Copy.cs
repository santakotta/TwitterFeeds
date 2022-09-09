using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Builder;

//using Newtonsoft.Json;

namespace TwitterFeeds
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /**
        private static void ConfigureServices(IServiceCollection services)
        {
            //we will configure logging here
            //services.AddLogging(configure => configure.AddConsole)
            //   .AddTransient<TweetReader>()
            //   .AddTransient<TweetProcessor>();
            services.AddLogging();
        }*/
        public void Configure(IApplicationBuilder app,ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
        }
        struct TempData
        {
            string capital;
        }
        public static async Task Main(string[] args)
        {
           

            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            // var serviceCollection = new ServiceCollection();
            // ConfigureServices(serviceCollection);

            //  var serviceProvider = serviceCollection.BuildServiceProvider();

            //  var logger = serviceProvider.GetService<ILogger<Program>>();
            //  var readerLogger = serviceProvider.GetService<ILogger<TweetReader>>();
            // var processorLogger = serviceProvider.GetService<ILogger<TweetProcessor>>();
            //var tweetReaderService = serviceProvider.GetService<TweetReader>();
            //var tweetProcessorService = serviceProvider.GetService<TweetProcessor>();
            //var tweetQueueService = serviceProvider.GetService<TweetQueue>();

            //_log.Info("Started Main");

            // logger.LogInformation("Program Started");

            //TweetQueue tweetQueue = new TweetQueue();
            //List<Task> tasks = new List<Task>();
            //TweetReader tweetReader = new TweetReader( tweetQueue);
            //tasks.Add(Task.Run(() => tweetReader.StartReadingTweets()));
            //Console.WriteLine("About to go to process tweets");
            //TweetProcessor tweetProcessor = new TweetProcessor( tweetQueue);
            //tasks.Add(Task.Run(() => tweetProcessor.InitializeAndStart()));
            //await Task.WhenAll(tasks);



            #region Previous working code

            //TweetQueue tweetQueue = new TweetQueue();
            //List<Task> tasks = new List<Task>();
            //TweetReader tweetReader = new TweetReader(tweetQueue);
            //tasks.Add(Task.Run(() => tweetReader.StartReadingTweets()));
            //Console.WriteLine("About to go to process tweets");
            //TweetProcessor tweetProcessor = new TweetProcessor(tweetQueue);
            //tasks.Add(Task.Run(() => tweetProcessor.InitializeAndStart()));
            //await Task.WhenAll(tasks);
            #endregion

            /** HttpClient _httpClient = new HttpClient();
             JsonSerializerOptions _options = new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             };
             Encoding encode = Encoding.GetEncoding("utf-8");
             _httpClient.DefaultRequestHeaders.Clear();
             
             _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer "+ "AAAAAAAAAAAAAAAAAAAAAPDogQEAAAAA99Cu8mAaR6xtZJfsKINdOLMIRSc%3D4HFnlyICSopWvuBFnmJcaZujKtRCMP7uDP2kOBmadSNpr9M1aD");
             try
             {
                 // var response = await _httpClient.GetStringAsync("https://api.twitter.com/2/tweets/sample/stream");
                 Uri uriObj = new Uri("https://api.twitter.com/2/tweets/sample/stream");
                 var response = await _httpClient.GetStreamAsync(uriObj);
                 StreamReader streamReader = new StreamReader(response, encode);
                 while(true)
                 {
                     Console.WriteLine(streamReader.ReadLine());
                 }
                 //  var response = await _httpClient.GetAsync("https://api.twitter.com/2/tweets/sample/stream");

                 //  if (response.IsSuccessStatusCode)
                 // {
                 //    Console.WriteLine("I am good");
                 // }
                 //if (response.IsSuccessStatusCode)
                 //  {

                 //using (var jsonReader = new JsonTextReader(streamReader))
                 //{
                 //    var serializer = new Newtonsoft.Json.JsonSerializer();
                 //    var obj = serializer.Deserialize(jsonReader);
                 //    //do some deserializing http://www.newtonsoft.com/json/help/html/Performance.htm
                 //}
                 // }
                 Console.WriteLine("After the call");
             }
             catch (Exception ex)
             {
                 throw ex;
             }
            */


        }
    }
}

