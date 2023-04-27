using System.ComponentModel.DataAnnotations.Schema;
using KfnApi.Abstractions;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Models.Entities;

public class ApprovalForm : IAuditable, IStateful<ApprovalFormState>
{
    public required Guid Id { get; set; }
    [ForeignKey(nameof(User))]
    public required Guid UserId { get; set; }
    public required string ProducerName { get; set; }
    public required List<string> Locations { get; set; }
    public required TimeOnly OpeningTime { get; set; }
    public required TimeOnly ClosingTime { get; set; }
    public required ApprovalFormState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Database Relations
    public User? User { get; set; }
    public List<Upload>? Uploads { get; set; }
}
