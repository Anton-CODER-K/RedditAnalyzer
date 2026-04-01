using RedditAnalyzer.Clients;
using RedditAnalyzer.Models;

namespace RedditAnalyzer.Service
{
    public class RedditService
    {
        private readonly RedditClient _client;
        private readonly ILogger<RedditService> _logger;

        public RedditService(RedditClient client, ILogger<RedditService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<PostResult>>> AnalyzeAsync(RequestModel request)
        {
            _logger.LogInformation("Starting analysis...");

            var result = new Dictionary<string, List<PostResult>>();

            foreach (var item in request.Items)
            {
                _logger.LogInformation("Processing subreddit {Subreddit}", item.Subreddit);
                var posts = await _client.GetPosts(item.Subreddit, request.Limit);


                var filtered = posts
                 .Where(p => item.Keywords.Any(k =>
                     (p.Title != null && p.Title.Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                     (p.Selftext != null && p.Selftext.Contains(k, StringComparison.OrdinalIgnoreCase))
                 ))
                 .Select(p => new PostResult
                 {
                     Title = p.Title,
                     HasMedia =
                         p.IsVideo ||
                         (p.Url?.Contains("i.redd.it") == true) ||
                         (p.Url?.Contains("v.redd.it") == true) ||
                         (p.Url?.Contains("reddit.com/gallery") == true) ||
                         (p.Url?.EndsWith(".jpg") == true) ||
                         (p.Url?.EndsWith(".png") == true) ||
                         (p.UrlOverriddenByDest?.Contains("i.redd.it") == true)
                 })
                 .ToList();



                result[$"/{item.Subreddit}"] = filtered;
                _logger.LogInformation("Filtered {Count} posts for {Subreddit}", filtered.Count, item.Subreddit);
            }

            return result;
        }
    }
}
