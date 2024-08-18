using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly ILogger<UrlsController> _logger;

        public UrlsController(IUrlService urlService, ILogger<UrlsController> logger)
        {
            _urlService = urlService;
            _logger = logger;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            _logger.LogInformation("ShortenUrl called with OriginalUrl: {OriginalUrl} and ExpirationOption: {ExpirationOption}", request.OriginalUrl, request.ExpirationOption);

            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            {
                return BadRequest("Original URL is required.");
            }

            try
            {
                var shortCode = await _urlService.ShortenUrlAsync(request.OriginalUrl, request.ExpirationOption);
                return Ok(shortCode);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid URL provided: {OriginalUrl}", request.OriginalUrl);
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode)
        {
            _logger.LogInformation("RedirectToOriginalUrl called with ShortCode: {ShortCode}", shortCode);

            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return BadRequest("Short code is required.");
            }

            var originalUrl = await _urlService.GetOriginalUrlByShortCodeAsync(shortCode);

            if (string.IsNullOrEmpty(originalUrl))
            {
                _logger.LogWarning("No original URL found for ShortCode: {ShortCode}", shortCode);
                return NotFound("Original URL not found.");
            }

            return Redirect(originalUrl);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetShortenedUrls(int page = 1, int pageSize = 100)
        {
            _logger.LogInformation("GetShortenedUrls called with page {Page} and pageSize {PageSize}", page, pageSize);

            var paginatedResult = await _urlService.GetPaginatedShortenedUrlsAsync(page, pageSize);

            if (paginatedResult == null || !paginatedResult.Data.Any())
            {
                _logger.LogWarning("No shortened URLs found.");
                return NotFound("No shortened URLs found.");
            }

            return Ok(paginatedResult);
        }


        [HttpDelete("{shortCode}")]
        public async Task<IActionResult> DeleteShortenedUrl(string shortCode)
        {
            _logger.LogInformation("DeleteShortenedUrl called with ShortCode: {ShortCode}", shortCode);

            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return BadRequest("Short code is required.");
            }

            var result = await _urlService.DeleteShortenedUrlAsync(shortCode);

            if (!result)
            {
                _logger.LogWarning("No shortened URL found for ShortCode: {ShortCode}", shortCode);
                return NotFound("Shortened URL not found.");
            }

            return NoContent(); // 204 No Content, indicates successful deletion
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshDatabase()
        {
            await _urlService.RefreshDatabaseAsync();
            return NoContent(); // Return 204 No Content to indicate success without response body
        }

    }
}