using Newtonsoft.Json;
using RedditAnalyzer.Models;
using System.Text.Json.Serialization;

namespace RedditAnalyzer.Clients
{
    public class RedditJsonClient : IRedditClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RedditJsonClient> _logger;

        public RedditJsonClient(HttpClient httpClient, ILogger<RedditJsonClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "RedditAnalyzer/1.0 (by u/anton_dev)"
            );
        }

        public async Task<List<RedditPost>> GetPosts(string subreddit, int limit)
        {
            try
            {

                var url = $"https://www.reddit.com/{subreddit}.json?limit={limit}";

                _logger.LogInformation("Fetching posts from {Url}", url);



                var response = await _httpClient.GetAsync(url);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error fetching subreddit {Subreddit}. Status: {StatusCode}", subreddit, response.StatusCode);
                    return new List<RedditPost>();
                }

                var data = JsonConvert.DeserializeObject<RedditResponse>(content);

                if (data?.Data?.Children == null)
                {
                    _logger.LogError("Invalid response structure for {Subreddit}", subreddit);
                    return new List<RedditPost>();
                }



                return data.Data.Children.Select(x => x.Data).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching subreddit {Subreddit}", subreddit);
                return new List<RedditPost>();
            }
        }
    }
}
