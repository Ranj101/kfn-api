namespace KfnApi.DTOs.Responses;

public sealed record ProducerPageResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
    public required DateTime CreatedAt { get; init; }
    public List<string> Reviews { get; set; } = new();
    public List<string> Gallery { get; set; } = new();
}
