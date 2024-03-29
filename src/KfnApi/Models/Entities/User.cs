﻿using KfnApi.Abstractions;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Models.Entities;

public sealed record User : IAuditable, IStateful<UserState>
{
    public required Guid Id { get; set; }
    public required string IdentityId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public Guid? ProfilePicture { get; set; }
    public Guid? CoverPicture { get; set; }
    public List<string> Roles { get; set; } = new();
    public required UserState State { get; set; }

    public required Guid CreatedBy { get; init; }
    public Guid? UpdatedBy { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Database Relations
    public Producer? Producer { get; set; }
    public List<Order>? Orders { get; set; }
    public List<Upload>? Uploads { get; set; }
    public List<UserReport>? AbuseReports { get; set; }
    public List<ApprovalForm>? ApprovalForms { get; set; }
}
