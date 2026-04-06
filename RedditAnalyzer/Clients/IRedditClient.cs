using RedditAnalyzer.Models;

namespace RedditAnalyzer.Clients
{
    public interface IRedditClient
    {
        Task<List<RedditPost>> GetPosts(string subreddit, int limit);
    }
}
