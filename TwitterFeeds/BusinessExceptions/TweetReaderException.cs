using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds
{
    public class TweetReaderException:Exception
    {
        public TweetReaderException(string message) : base(message)
        {
        }

        public TweetReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
