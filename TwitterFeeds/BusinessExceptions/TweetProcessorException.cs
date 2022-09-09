using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterFeeds.BusinessExceptions
{
    public class TweetProcessorException : Exception
    {
        public TweetProcessorException(string message) : base(message)
        {
        }

        public TweetProcessorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
