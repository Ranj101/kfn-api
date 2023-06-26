namespace KfnApi.DTOs.Responses;

public sealed record ProfileResponse
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string? CoverPicture { get; set; }
    public required string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }
}
