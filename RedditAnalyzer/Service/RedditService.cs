using RedditAnalyzer.Clients;
using RedditAnalyzer.Models;

namespace RedditAnalyzer.Service
{
    public class RedditService
    {
        private readonly RedditJsonClient _clientJson;
        private readonly RedditHtmlClient _clientHtml;
        private readonly RedditPlaywrightClient _clientPlaywright;  
        private readonly ILogger<RedditService> _logger;

        public RedditService(RedditJsonClient clientJson, RedditHtmlClient clientHtml, RedditPlaywrightClient playwrightClient, ILogger<RedditService> logger)
        {
            _clientJson = clientJson;
            _clientHtml = clientHtml;
            _clientPlaywright = playwrightClient;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<PostResult>>> AnalyzeJsonAsync(RequestModel request)
        {
            _logger.LogInformation("Starting analysis...");

            var result = new Dictionary<string, List<PostResult>>();

            foreach (var item in request.Items)
            {
                _logger.LogInformation("Processing subreddit {Subreddit}", item.Subreddit);
                var posts = await _clientJson.GetPosts(item.Subreddit, request.Limit);


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


        public async Task<Dictionary<string, List<PostResult>>> AnalyzeHtmlAsync(RequestModel request)
        {
            _logger.LogInformation("Starting analysis...");

            var result = new Dictionary<string, List<PostResult>>();

            foreach (var item in request.Items)
            {
                _logger.LogInformation("Processing subreddit {Subreddit}", item.Subreddit);
                var posts = await _clientHtml.GetPosts(item.Subreddit, request.Limit, item.Keywords);

                result[$"/{item.Subreddit}"] = posts.Select(p => new PostResult
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
                }).ToList();
            }

            return result;
        }

        public async Task<Dictionary<string, List<PostResult>>> AnalyzePlaywrightAsync(RequestModel request)
        {
            _logger.LogInformation("Starting Playwright analysis...");

            var result = new Dictionary<string, List<PostResult>>();

            foreach(var item in request.Items)
            {
                _logger.LogInformation("Processing subreddit {Subreddit}", item.Subreddit);
                var posts = await _clientPlaywright.GetPost(item.Subreddit, request.Limit, item.Keywords);
                result[$"/{item.Subreddit}"] = posts.Select(p => new PostResult
                {
                    Title = p.Title,
                    HasMedia =
                        p.IsVideo ||
                        (p.Domain?.Contains("i.redd.it") == true) ||
                        (p.Domain?.Contains("v.redd.it") == true) ||
                        (p.Domain?.Contains("rgallery") == true) ||
                        (p.Url?.EndsWith(".jpg") == true) ||
                        (p.Url?.EndsWith(".png") == true) ||
                        (p.UrlOverriddenByDest?.Contains("i.redd.it") == true)
                }).ToList();
            }

            return result;
        }
    }
}
