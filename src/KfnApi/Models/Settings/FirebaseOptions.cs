using Microsoft.Build.Framework;

namespace KfnApi.Models.Settings;

public sealed record FirebaseOptions
{
    public const string SectionName = "Firebase";

    [Required]
    public required string Type { get; set; }

    [Required]
    public required string ProjectId { get; set; }

    [Required]
    public required string PrivateKeyId { get; set; }

    [Required]
    public required string PrivateKey { get; set; }

    [Required]
    public required string ClientEmail { get; set; }

    [Required]
    public required string ClientId { get; set; }

    [Required]
    public required string AuthUri { get; set; }

    [Required]
    public required string TokenUri { get; set; }

    [Required]
    public required string AuthProviderX509CertUrl { get; set; }

    [Required]
    public required string ClientX509CertUrl { get; set; }

    [Required]
    public required string UniverseDomain {get; set; }
}
