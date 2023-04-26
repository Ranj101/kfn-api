using System.ComponentModel.DataAnnotations;

namespace KfnApi.DTOs.Requests;

public sealed record CreateProductRequest
{
    [Required]
    [MaxLength(350)]
    public required string Name { get; set; }

    public Guid? Picture { get; set; }

    [Required]
    [MinLength(1), MaxLength(10)]
    public required List<CreatePriceByWeightRequest> Prices { get; set; }
}
