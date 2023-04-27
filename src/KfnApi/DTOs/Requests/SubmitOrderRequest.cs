using System.ComponentModel.DataAnnotations;

namespace KfnApi.DTOs.Requests;

public sealed record SubmitOrderRequest
{
    [Required]
    public required Guid ProducerId { get; set; }

    [Required]
    public required string Location { get; set; }

    [Required]
    public required DateTime PickupTime { get; set; }

    [Required]
    public required List<OrderedProductRequest> Products { get; set; }
}
