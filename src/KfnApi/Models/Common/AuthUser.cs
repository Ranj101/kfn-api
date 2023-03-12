using System.Text.Json.Serialization;

namespace KfnApi.Models.Common;

public sealed record AuthUser
{
    [JsonPropertyName("user_id")]
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required string Nickname { get; set; }
    public string Picture { get; set; } = string.Empty;
    public required string Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public List<string> Roles { get; set; } = new();

    public List<Identity> Identities { get; set; } = new();

    [JsonPropertyName("last_ip")]
    public string LastIp { get; set; } = string.Empty;

    [JsonPropertyName("last_login")]
    public DateTime LastLogin { get; set; }

    [JsonPropertyName("logins_count")]
    public int LoginsCount { get; set; }
}

public sealed record Identity
{
    public bool IsSocial { get; set; }
    [JsonPropertyName("user_id")]
    public required string UserId { get; set; }
    public required string Provider { get; set; }
    public required string Connection { get; set; }
}
