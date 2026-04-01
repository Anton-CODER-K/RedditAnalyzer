using Newtonsoft.Json;

namespace RedditAnalyzer.Models
{
    public class RedditPost
    {
        public string Title { get; set; }
        public string Selftext { get; set; }
        public string Url { get; set; }
        public string UrlOverriddenByDest { get; set; }
        public bool IsVideo { get; set; }
        public string Domain { get; set; }

    }

    public class PostResult
    {
        public string Title { get; set; }
        public bool HasMedia { get; set; }
    }
}
