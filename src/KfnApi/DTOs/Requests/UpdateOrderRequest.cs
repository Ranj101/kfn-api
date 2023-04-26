using Microsoft.Build.Framework;

namespace KfnApi.DTOs.Requests;

public sealed record UpdateOrderRequest
{
    [Required]
    public required string Location { get; set; }

    [Required]
    public required DateTime PickupTime { get; set; }

    [Required]
    public required List<OrderedProductRequest> Products { get; set; }
}
