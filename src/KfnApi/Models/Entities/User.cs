using KfnApi.Abstractions;
using KfnApi.Models.Enums.Workflow;

namespace KfnApi.Models.Entities;

public sealed record User : IAuditable
{
    public required Guid Id { get; set; }
    public required string IdentityId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public List<string> Providers { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public List<string> AbuseReports { get; set; } = new();
    public required UserState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Database Relations
    public Producer Producer { get; set; }
    public ProducerApprovalForm ProducerApprovalForm { get; set; }
    public List<Order> Orders { get; set; }
}
