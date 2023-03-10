namespace KfnApi.Models.Common;

public sealed record AuthUser
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required string Nickname { get; set; }
    public required string Username { get; set; }
    public required string Picture { get; set; }
    public required string Email { get; set; }
    public bool EmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Identity> Identities { get; set; } = new();
    public AppMetadata AppMetadata { get; set; } = new();

    public required  string LastIp { get; set; }
    public DateTime LastLogin { get; set; }
    public int LoginsCount { get; set; }
}

public sealed record Identity
{
    public bool IsSocial { get; set; }
    public required string UserId { get; set; }
    public required string Provider { get; set; }
    public required string Connection { get; set; }
}

public sealed record AppMetadata
{
    public List<string> Roles { get; set; } = new();
}
