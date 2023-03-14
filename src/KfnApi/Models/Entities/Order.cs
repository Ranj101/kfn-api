using System.ComponentModel.DataAnnotations.Schema;
using KfnApi.Abstractions;

namespace KfnApi.Models.Entities;

public sealed record Order : IAuditable
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(User))]
    public required Guid UserId { get; set; }
    [ForeignKey(nameof(Producer))]
    public required Guid ProducerId { get; set; }

    public required string State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    public required double TotalPrice { get; set; }
    public required string Location { get; set; }
    public required DateTime PickupTime { get; set; }

    public User User { get; set; }
    public Producer Producer { get; set; }
    public List<Product> Products { get; set; }
}
