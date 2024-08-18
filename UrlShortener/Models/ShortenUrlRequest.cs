namespace UrlShortener.Api.Models
{
    public class ShortenUrlRequest
    {
        public string OriginalUrl { get; set; }
        public string ExpirationOption { get; set; } = "1 day"; // Options: "1 day", "1 week", "indefinite"
    }
}