namespace RedditAnalyzer.Models
{
    public class RequestModel
    {
        public List<SubredditItem> Items { get; set; }
        public int Limit { get; set; }
    }
}
