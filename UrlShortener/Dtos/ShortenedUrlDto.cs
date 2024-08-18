namespace UrlShortener.Dtos
{
    public class ShortenedUrlDto
    {
        public string ShortCode { get; set; }
        public string OriginalUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ExpirationOption { get; set; } = "1 day"; // Options: "1 day", "1 week", "indefinite"
    }

}
