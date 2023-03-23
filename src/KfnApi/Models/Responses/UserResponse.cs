﻿using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Models.Responses;

public sealed record UserResponse
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
}
