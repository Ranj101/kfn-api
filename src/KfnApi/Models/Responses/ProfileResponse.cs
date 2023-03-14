namespace KfnApi.Models.Responses;

public sealed record ProfileResponse
{
    public required string Id { get; set; }
    public required string Email { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> Providers { get; set; } = new();
}
