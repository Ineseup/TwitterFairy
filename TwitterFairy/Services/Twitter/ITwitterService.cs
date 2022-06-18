using TwitterFairy.Models;

namespace TwitterFairy.Services.Twitter
{
    public interface ITwitterService
    {
        public Task<List<TwitterOutput>> GetTweets(string query);
        public string CreateQuery(string[] keywords, string[] accountsFrom, string[] hashtags, string[] cashtags);
    }
}
