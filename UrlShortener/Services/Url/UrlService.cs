using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;
using UrlShortener.Dtos;

namespace UrlShortener.Api.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext _context;
        private readonly IShortCodeGenerator _shortCodeGenerator;

        public UrlService(ApplicationDbContext context, IShortCodeGenerator shortCodeGenerator)
        {
            _context = context;
            _shortCodeGenerator = shortCodeGenerator;
        }

        // A method to validate URLs
        private bool IsValidUrl(string url)
        {
            // This pattern matches www.example.com, example.com, https://example.com, and http://example.com
            string pattern = @"^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+\.[a-zA-Z]{2,}(\.[a-zA-Z]{2,})?(/.*)?$";
            return Regex.IsMatch(url, pattern);
        }
        private string NormalizeUrl(string url)
        {
            // If the URL doesn't start with http:// or https://, prepend http://
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }
            return url;
        }

        public async Task<string> ShortenUrlAsync(string originalUrl, string expirationOption = "1 day")
        {
            if (!IsValidUrl(originalUrl))
            {
                throw new ArgumentException("The provided URL is not in a valid format.");
            }

            originalUrl = NormalizeUrl(originalUrl);

            // Check for existing URL
            var existingUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(url => url.OriginalUrl == originalUrl);
            if (existingUrl != null)
            {
                return existingUrl.ShortCode;
            }

            string shortCode;
            do
            {
                shortCode = _shortCodeGenerator.GenerateShortCode();
            } while (await _context.ShortenedUrls.AnyAsync(url => url.ShortCode == shortCode));

            Console.WriteLine($"Received expiration option: {expirationOption}");

            DateTime? expirationTime = expirationOption.ToLower() switch
            {
                "1 day" => DateTime.UtcNow.AddDays(1),
                "1 week" => DateTime.UtcNow.AddDays(7),
                "indefinite" => null,
                _ => DateTime.UtcNow.AddDays(1) // Default to 1 day
            };

            Console.WriteLine($"Calculated expiration time: {expirationTime}");

            var shortenedUrl = new ShortenedUrl
            {

                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ExpirationTime = expirationTime
            };

            _context.ShortenedUrls.Add(shortenedUrl);
            await _context.SaveChangesAsync();

            return shortCode;
        }

        public async Task<string> GetOriginalUrlByShortCodeAsync(string shortCode)
        {
            var urlEntry = await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            // Check if the URL has expired
            if (urlEntry == null || (urlEntry.ExpirationTime.HasValue && urlEntry.ExpirationTime.Value < DateTime.UtcNow))
            {
                return null; // URL not found or expired
            }

            return urlEntry.OriginalUrl;
        }

        public async Task<IEnumerable<ShortenedUrlDto>> GetAllShortenedUrlsAsync()
        {
            var urlEntries = await _context.ShortenedUrls.ToListAsync();

            return urlEntries.Select(url => new ShortenedUrlDto
            {
                ShortCode = url.ShortCode,
                OriginalUrl = url.OriginalUrl,
                CreatedDate = url.CreatedAt
            });
        }

        public async Task<bool> DeleteShortenedUrlAsync(string shortCode)
        {
            var url = await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (url == null)
            {
                return false;
            }

            _context.ShortenedUrls.Remove(url);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task RefreshDatabaseAsync()
        {
            var expiredUrls = _context.ShortenedUrls.Where(u => u.ExpirationTime.HasValue && u.ExpirationTime.Value < DateTime.UtcNow);
            _context.ShortenedUrls.RemoveRange(expiredUrls);
            await _context.SaveChangesAsync();
        }
        public async Task<PaginatedResult<ShortenedUrlDto>> GetPaginatedShortenedUrlsAsync(int page, int pageSize)
        {
            var totalEntries = await _context.ShortenedUrls.CountAsync();
            var urlEntries = await _context.ShortenedUrls
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResult<ShortenedUrlDto>
            {
                TotalEntries = totalEntries,
                TotalPages = (int)Math.Ceiling(totalEntries / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = urlEntries.Select(url => new ShortenedUrlDto
                {
                    ShortCode = url.ShortCode,
                    OriginalUrl = url.OriginalUrl,
                    CreatedDate = url.CreatedAt,
                    ExpirationOption = "1 day" // Assuming the default or specified expiration option is handled here
                }).ToList()
            };

            return result;
        }

    }
}
