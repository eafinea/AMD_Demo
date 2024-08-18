using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Api.Models
{
    public class ShortenedUrl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationTime { get; set; } // Nullable DateTime for expiration
    }
}