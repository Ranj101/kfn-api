namespace KfnApi.Models.Responses;

public sealed record ProducerPageResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
    public List<string> Reviews { get; set; } = new();
    public required DateTime CreatedAt { get; init; }
}
