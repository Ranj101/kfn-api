using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Responses;

public sealed record FormListResponse
{
    public required Guid Id { get; set; }
    public required string ProducerName { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
    public required ApprovalFormState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
}
