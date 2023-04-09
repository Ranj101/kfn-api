namespace KfnApi.DTOs.Responses;

public sealed record ProducerPageListResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
}
