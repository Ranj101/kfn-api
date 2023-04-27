using System.ComponentModel.DataAnnotations;

namespace KfnApi.DTOs.Requests;

public sealed record OrderedProductRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public Guid PriceByWeightId { get; set; }
}
