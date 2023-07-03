
namespace KfnApi.DTOs.Responses;

public sealed record SelfResponse
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? CoverPicture { get; set; }
    public string? ProfilePicture { get; set; }
    public List<string> Roles { get; set; } = new();
    public Guid? ProducerId { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
}
