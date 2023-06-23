using System.ComponentModel.DataAnnotations;

namespace IntegrationTests.Models;

public record TokenSettings
{
    public const string SectionName = "TokenSettings";

    [Required]
    public required string Audience { get; set; }

    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required int AccessTokenExpiration { get; set; }

    [Required]
    public required string AccessTokenSecret { get; set; }
}
