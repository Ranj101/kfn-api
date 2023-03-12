namespace KfnApi.Models.Entities;

public sealed record User
{
    public required string Id { get; set; }

    public required string Email { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<string> Providers { get; set; } = new();

    public List<string> Roles { get; set; } = new();
}
