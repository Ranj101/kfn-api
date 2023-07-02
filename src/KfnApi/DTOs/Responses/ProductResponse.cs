using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Responses;

public sealed record ProductResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid ProducerId { get; set; }
    public required string ProducerName { get; set; }
    public required string Picture { get; set; }
    public required ProductState State { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public required List<PriceByWeightResponse> PricesByWeight { get; set; }
}
