using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitterFairy.Models;
using TwitterFairy.Services.Twitter;

namespace TwitterFairy.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class TwitterController : ControllerBase
    {
        private readonly ILogger<TwitterController> _logger;
        private readonly ITwitterService _twitterService;

        public TwitterController(ILogger<TwitterController> logger, ITwitterService twitterService)
        {
            _logger = logger;
            _twitterService = twitterService;
        }

        [HttpPost]
        public async Task<List<TwitterOutput>> Post(TwitterInput dto)
        {
            var query = _twitterService.CreateQuery(dto.Keywords, dto.AccountsFrom, dto.Hashtags, dto.Cashtags);
            var tweets = await _twitterService.GetTweets(query);
            return tweets;   
        }
    }
}