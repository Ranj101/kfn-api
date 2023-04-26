namespace KfnApi.DTOs.Responses;

public sealed record PriceByWeightResponse
{
    public required Guid Id { get; set; }
    public required double Value { get; set; }
    public required double Weight { get; set; }
}
