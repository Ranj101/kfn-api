using System.ComponentModel.DataAnnotations;

namespace KfnApi.DTOs.Responses;

public sealed record PriceByWeightResponse
{
    [Required]
    [Range(0, double.MaxValue)]
    public required double Value { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public required double Weight { get; set; }
}
