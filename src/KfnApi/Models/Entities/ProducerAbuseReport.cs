using System.ComponentModel.DataAnnotations.Schema;
using KfnApi.Abstractions;

namespace KfnApi.Models.Entities;

public sealed record ProducerAbuseReport : IAbuseReport
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(Producer))]
    public required Guid ProducerId { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Database Relations
    public Producer? Producer { get; set; }
}
