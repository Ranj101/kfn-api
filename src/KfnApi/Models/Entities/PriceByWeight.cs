using System.ComponentModel.DataAnnotations.Schema;

namespace KfnApi.Models.Entities;

public sealed record PriceByWeight
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(Product))]
    public required Guid ProductId { get; set; }
    public required double Value { get; set; }
    public required double Weight { get; set; }

    // Database Relations
    public Product Product { get; set; }
}
