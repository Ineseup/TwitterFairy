namespace TwitterFairy.Models
{
    public class TwitterInput
    {
        public string[] AccountsFrom { get; set; }
        public string[] Keywords { get; set; }
        public string[] Hashtags { get; set; }
        public string[] Cashtags { get; set; }
    }

    public class TwitterOutput
    {
        public bool PositiveSentiment { get; set; }
        public string TweetTime { get; set; }
        public string[] Coins { get; set; }
        public int IntervalMinutes { get; set; }
        public string TweetText { get; set; }
    }
}
