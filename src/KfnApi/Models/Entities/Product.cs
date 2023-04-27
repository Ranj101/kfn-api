using System.ComponentModel.DataAnnotations.Schema;
using KfnApi.Abstractions;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Models.Entities;

public sealed record Product : IAuditable, IStateful<ProductState>
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(Producer))]
    public required Guid ProducerId { get; set; }
    public required string Name { get; set; }
    public required Guid Picture { get; set; }
    public required ProductState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Database Relations
    public Producer? Producer { get; set; }
    public List<Order>? Orders { get; set; }
    public List<Upload>? Uploads { get; set; }
    public List<PriceByWeight>? Prices { get; set; }
}
