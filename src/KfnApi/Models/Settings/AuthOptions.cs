using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Settings;

public sealed record AuthOptions
{
    public const string SectionName = "Auth";

    [Url]
    [Required]
    public required string AuthorityUrl { get; set; }

    [Url]
    [Required]
    public required string WrapperUrl { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    public required string M2MClientId { get; set; }

    [Required]
    public required string M2MClientSecret { get; set; }
}

public static class AuthDefaults
{
    public const string IdentityClient = "AuthZeroClient";
    public const string IdentityWrapperClient = "AuthZeroManagementApiClient";
}
