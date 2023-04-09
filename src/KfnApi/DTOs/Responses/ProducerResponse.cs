using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Responses;

public sealed record ProducerResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
    public required ProducerState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<string> Reviews { get; set; } = new();
    public List<string> Gallery { get; set; } = new();
}
