using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TwitterFeeds
{
    public class TweetDetail
    {
        public TweetDetail():base()
        {

        }
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime EnqueueTimeStamp { get; set; }
        public bool IsProcessed { get; set; }
    }
    public class Error
    {
        public string Title { get; set; }
        [JsonPropertyName("disconnect_type")]
        public string DisconnectType { get; set; }
        public string Detail { get; set; }
        public string Type { get; set; }
    }

    public class Tweet
    {
        [JsonPropertyName("Data")]
        public TweetDetail Detail { get; set; }
        public List<Error> Errors { get; set; }

    }
}
