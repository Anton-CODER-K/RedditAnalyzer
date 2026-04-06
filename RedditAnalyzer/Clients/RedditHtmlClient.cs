using HtmlAgilityPack;
using RedditAnalyzer.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RedditAnalyzer.Clients
{
    public class RedditHtmlClient
    {
        private readonly HttpClient _httpClient;

        public RedditHtmlClient(HttpClient http)
        {
            _httpClient = http;
            _httpClient.DefaultRequestHeaders.Add(

                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36"
            );
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        }

        public async Task<List<RedditPost>> GetPosts(string subreddit, int limit, List<SubredditItem> items)
        {
            var posts = new List<RedditPost>();

            var keywords = items
                .SelectMany(i => i.Keywords)
                .ToList();

            var url = $"https://old.reddit.com/{subreddit}/";

            while (posts.Count < limit && url != null)
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = CreateDocument(html);

                var parsedPosts = ParsePosts(doc);

                var filtered = parsedPosts
                  .Where(p => keywords.Any(k =>
                      (p.Title != null && p.Title.Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                      (p.Selftext != null && p.Selftext.Contains(k, StringComparison.OrdinalIgnoreCase))
                  )).ToList();

                posts.AddRange(filtered);

                url = GetNextPageUrl(doc);

                Console.WriteLine($"Loaded: {posts.Count}, Next: {url}");
            }

            return posts.Take(limit).ToList();
        }


        private string? GetNextPageUrl(HtmlDocument doc)
        {
            return doc.DocumentNode
                .SelectSingleNode("//span[@class='next-button']/a")
                ?.GetAttributeValue("href", null);
        }


        private HtmlDocument CreateDocument(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        private List<RedditPost> ParsePosts(HtmlDocument doc)
        {
            var posts = new List<RedditPost>();

            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'thing')]");

            if (nodes == null)
                return posts;

            foreach (var node in nodes)
            {
                var titleNode = node.SelectSingleNode(".//a[@data-event-action='title']");

                var title = HtmlEntity.DeEntitize(titleNode?.InnerText ?? "").Trim();

                var url = node.GetAttributeValue("data-url", "");
                var domain = node.GetAttributeValue("data-domain", "");
                var post = new RedditPost
                {
                    Title = title,
                    Selftext = "",
                    Url = url,
                    UrlOverriddenByDest = url,
                    IsVideo = domain.Contains("v.redd.it"),
                    Domain = domain
                };
                posts.Add(post);

            }

            return posts;

        }





        //--------------------------------------------------------------


      
    }
}