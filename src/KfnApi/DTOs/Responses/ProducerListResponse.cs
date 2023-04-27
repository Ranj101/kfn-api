using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Responses;

public sealed record ProducerListResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required ProducerState State { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
}
