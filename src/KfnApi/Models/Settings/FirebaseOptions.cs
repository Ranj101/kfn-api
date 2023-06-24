using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Settings;

public sealed record FirebaseOptions
{
    public const string SectionName = "Firebase";

    public bool UseFirebase { get; set; } = true;

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

    [Url]
    [Required]
    public required string AuthUri { get; set; }

    [Url]
    [Required]
    public required string TokenUri { get; set; }

    [Url]
    [Required]
    public required string AuthProviderX509CertUrl { get; set; }

    [Url]
    [Required]
    public required string ClientX509CertUrl { get; set; }

    [Required]
    public required string UniverseDomain {get; set; }
}
