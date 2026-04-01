using Newtonsoft.Json;
using RedditAnalyzer.Models;
using System.Text.Json.Serialization;

namespace RedditAnalyzer.Clients
{
    public class RedditClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RedditClient> _logger;

        public RedditClient(HttpClient httpClient, ILogger<RedditClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<RedditPost>> GetPosts(string subreddit, int limit)
        {
           

            var url = $"https://www.reddit.com/{subreddit}.json?limit={limit}";

            _logger.LogInformation("Fetching posts from {Url}", url);

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "RedditApp/1.0");
            }

            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error fetching subreddit {Subreddit}. Status: {StatusCode}", subreddit, response.StatusCode);
                throw new Exception($"Reddit error: {response.StatusCode} | {content}");
            }

            var data = JsonConvert.DeserializeObject<RedditResponse>(content);

            if (data?.Data?.Children == null)
            {
                _logger.LogError("Invalid response structure for {Subreddit}", subreddit);
                throw new Exception("Invalid Reddit response");
            }

            

            return data.Data.Children.Select(x => x.Data).ToList();
        }
    }
}
