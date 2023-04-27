using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Responses;

public sealed record OrderResponse
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid ProducerId { get; set; }
    public required double TotalPrice { get; set; }
    public required string Location { get; set; }
    public required DateTime PickupTime { get; set; }
    public required OrderState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    public required List<ProductListResponse> Products { get; set; }
}
