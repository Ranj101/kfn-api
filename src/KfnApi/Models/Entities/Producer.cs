using System.ComponentModel.DataAnnotations.Schema;
using KfnApi.Abstractions;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Models.Entities;

public sealed record Producer : IAuditable, IStateful<ProducerState>
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(User))]
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
    public List<string> Reviews { get; set; } = new();
    public required ProducerState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Database Relations
    public User? User { get; set; }
    public List<Order>? Orders { get; set; }
    public List<ProducerAbuseReport>? AbuseReports { get; set; }
    public List<Product>? Products { get; set; }
}
