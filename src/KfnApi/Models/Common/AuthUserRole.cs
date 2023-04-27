namespace KfnApi.Models.Common;

public sealed record AuthUserRole
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
