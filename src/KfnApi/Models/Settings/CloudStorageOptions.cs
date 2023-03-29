using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Settings;

public sealed record CloudStorageOptions
{
    public const string SectionName = "CloudStorage" ;

    [Required]
    public required string Type { get; set; }

    [Required]
    public required string Bucket { get; set; }

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
    public required string AuthProviderCertUrl { get; set; }

    [Url]
    [Required]
    public required string ClientCertUrl { get; set; }
}
