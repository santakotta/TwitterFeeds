using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds
{
    public class TweetStatReporter
    {
        private DateTime _reportTime;
        public TweetStatReporter(DateTime reportTime)
        {
            this._reportTime = reportTime;
        }
        public void PrintErrorToConsole(string message)
        {
            PrintHeader();
            Console.WriteLine(message);
            PrintFooter();
        }
        private void PrintHeader()
        {
            Console.WriteLine(String.Format("..................Twitter Feeds Report@{0}..................", _reportTime));
        }
        public void PrintResultToConsole(List<String> results)
        {
            PrintHeader();
            results.ForEach(r => Console.WriteLine(r));
            PrintFooter();
        }
        private void PrintFooter()
        {
            Console.WriteLine("..................End Report..................");
        }

    }
}
