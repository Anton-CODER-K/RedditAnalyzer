using HtmlAgilityPack;
using Microsoft.Playwright;
using RedditAnalyzer.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace RedditAnalyzer.Clients
{
    public class RedditPlaywrightClient
    {
        public async Task<List<RedditPost>?> GetPost(string subreddit, int limit, List<string>? keywords)
        {

            var posts = new List<RedditPost>();
            var seen = new HashSet<string>();

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            
            });

            var page = await CreatePage(subreddit, browser);

            var html = await page.ContentAsync();



            //Console.WriteLine("=== HTML LOADED ===");
            //Console.WriteLine(html.Substring(1000));



            int lastCount = 0;
            int sameCount = 0;

            while (posts.Count < limit && sameCount < 3)
            {
                var elements = await page.QuerySelectorAllAsync("article");

                foreach (var el in elements)
                {
                    var titleEl = await el.QuerySelectorAsync("a[slot='title']");
                    if (titleEl == null) continue;

                    var text = await titleEl.InnerTextAsync();

                    if (seen.Contains(text)) continue;
                    seen.Add(text);

                    if (keywords == null || keywords.Count == 0 ||
                        keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        var title = await titleEl.InnerTextAsync();
                        var url = await titleEl.GetAttributeAsync("href");

                        var fullUrl = url != null
                            ? "https://www.reddit.com" + url
                            : "";

                        var domain = await el.GetAttributeAsync("domain");

                        var videoEl = await el.QuerySelectorAsync("video");

                        bool isVideo = videoEl != null;

                     

                        posts.Add(new RedditPost
                        {
                            Title = title,
                            Selftext = "",
                            Url = fullUrl,
                            UrlOverriddenByDest = url,
                            Domain = domain,
                            IsVideo = isVideo
                        });

                        if (posts.Count >= limit)
                            break;
                    }
                }

                if (posts.Count == lastCount)
                    sameCount++;
                else
                    sameCount = 0;

                lastCount = posts.Count;

                await page.Mouse.WheelAsync(0, 5000);
                await Task.Delay(2000);
            }

            await browser.CloseAsync();

            return posts;

        }


        public async Task<IPage> CreatePage(string subreddit, IBrowser browser)
        {
            var context = await browser.NewContextAsync(new()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36",
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                DeviceScaleFactor = 1,
                IsMobile = false,
                HasTouch = false,
                Locale = "uk-UA",
                TimezoneId = "Europe/Kyiv"
            });
            await context.AddInitScriptAsync(@"
                Object.defineProperty(navigator, 'webdriver', { get: () => undefined });
                window.chrome = { runtime: {} };
                Object.defineProperty(navigator, 'languages', { get: () => ['uk-UA', 'uk', 'en-US', 'en'] });
                Object.defineProperty(navigator, 'plugins', { get: () => [1, 2, 3, 4, 5] });
            ");
            var page = await context.NewPageAsync();
            await page.GotoAsync($"https://www.reddit.com/{subreddit}/");

            

            if (page.Url.Contains("captcha"))
            {
                Console.WriteLine("Captcha detected");
                throw new Exception("Captcha detected");
            }

            try
            {
                await page.WaitForSelectorAsync("article");
            }
            catch
            {
                throw new InvalidOperationException("Not Detected post");
            }

            var html = await page.ContentAsync();


            //Console.WriteLine("=== HTML LOADED ===");
            //Console.WriteLine(html.Substring(1000));

            return page;
        }

    }
}
