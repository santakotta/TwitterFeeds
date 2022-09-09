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
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static async Task Main(string[] args)
        {
            PrintStartBanner();
            Console.ReadKey();
            // Load configuration
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Log some things
            log.Info("Main method started.");

            try
            {
                TweetQueue tweetQueue = new TweetQueue();
                List<Task> tasks = new List<Task>();
                log.Info("Starting TweetReader task.");
                TweetReader tweetReader = new TweetReader(tweetQueue);
                tasks.Add(Task.Run(() => tweetReader.StartReadingTweets()));
                log.Info("Starting TweetProcessor task.");
                TweetProcessor tweetProcessor = new TweetProcessor(tweetQueue);
                tasks.Add(Task.Run(() => tweetProcessor.InitializeAndStart()));
                await Task.WhenAll(tasks);

                //Action readAction = () =>
                //{
                //    tweetReader.StartReadingTweets();
                //};
                ////Action processAction = () =>
                ////{
                ////    tweetProcessor.InitializeAndStart();
                ////};
                //Parallel.Invoke(readAction);
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            log.Info("From Main method:: All tasks done.");
        }

        private static void PrintStartBanner()
        {
            Console.WriteLine("***********************************");
            Console.WriteLine("*                                 *");
            Console.WriteLine("*   WELCOME TO TWEET PROCESSOR    *");
            Console.WriteLine(string.Format("*      {0}       *", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")));
            Console.WriteLine("*                                 *");
            Console.WriteLine("***********************************");
            Console.WriteLine("This program reads twitter sample API and outputs the number of tweets read and the top ten hashtags");
            Console.WriteLine("The logs can be found under the bin folder: bin/Debug/netcoreapp3.1/TwitterFeeds.log");
            Console.WriteLine("To stop the program press Ctrl+C");
            Console.WriteLine("press any key to start...");

        }
    }
}

