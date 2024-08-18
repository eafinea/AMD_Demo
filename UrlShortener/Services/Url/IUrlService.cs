using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Api.Models;
using UrlShortener.Dtos;

namespace UrlShortener.Api.Services
{
    public interface IUrlService
    {
        Task<string> ShortenUrlAsync(string originalUrl, string expirationOption = "1 day");
        Task<string> GetOriginalUrlByShortCodeAsync(string shortCode);
        Task<IEnumerable<ShortenedUrlDto>> GetAllShortenedUrlsAsync();
        Task<bool> DeleteShortenedUrlAsync(string shortCode);
        Task RefreshDatabaseAsync();
        Task<PaginatedResult<ShortenedUrlDto>> GetPaginatedShortenedUrlsAsync(int page, int pageSize);

    }
}
