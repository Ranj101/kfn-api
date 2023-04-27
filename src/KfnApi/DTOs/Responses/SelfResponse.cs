
namespace KfnApi.DTOs.Responses;

public sealed record SelfResponse
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string? CoverPicture { get; set; }
    public required string? ProfilePicture { get; set; }
    public List<string> Providers { get; set; } = new();
    public List<string> Roles { get; set; } = new();

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
}
