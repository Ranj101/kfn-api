using System.ComponentModel.DataAnnotations.Schema;
using KfnApi.Abstractions;

namespace KfnApi.Models.Entities;

public sealed record Product : IAuditable
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(Producer))]
    public required Guid ProducerId { get; set; }
    public required string Name { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    public Producer Producer { get; set; }
    public List<PriceByWeight> Prices { get; set; }
    public List<Order> Orders { get; set; }

}
