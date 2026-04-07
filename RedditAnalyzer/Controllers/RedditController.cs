using Microsoft.AspNetCore.Mvc;
using RedditAnalyzer.Models;
using RedditAnalyzer.Service;

namespace RedditAnalyzer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedditController : ControllerBase
    {
        private readonly RedditService _service;
        private readonly ILogger<RedditController> _logger;
        public RedditController(RedditService service, ILogger<RedditController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("analyze/json")]
        public async Task<IActionResult> AnalyzeJson([FromBody] RequestModel request)
        {
            _logger.LogInformation("Received request: {@Request}", request);

            var result = await _service.AnalyzeJsonAsync(request);

            _logger.LogInformation("Returning result with {Count} subreddits", result.Count);

            return Ok(result);
        }


        [HttpPost("analyze/html")]
        public async Task<IActionResult> AnalyzeHtml([FromBody] RequestModel request)
        {
            _logger.LogInformation("Received request: {@Request}", request);

            var result = await _service.AnalyzeHtmlAsync(request);

            _logger.LogInformation("Returning result with {Count} subreddits", result.Count);

            return Ok(result);
        }

        [HttpPost("analyze/Playwright")]
        public async Task<IActionResult> AnalyzePlaywright([FromBody] RequestModel request)
        {
            _logger.LogInformation("Received request: {@Request}", request);
            var result = await _service.AnalyzePlaywrightAsync(request);
            _logger.LogInformation("Returning result with {Count} subreddits", result.Count);
            return Ok(result);
        }
    }
}