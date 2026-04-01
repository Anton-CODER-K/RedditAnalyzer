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

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] RequestModel request)
        {
            _logger.LogInformation("Received request: {@Request}", request);

            var result = await _service.AnalyzeAsync(request);

            _logger.LogInformation("Returning result with {Count} subreddits", result.Count);

            return Ok(result);
        }
    }
}