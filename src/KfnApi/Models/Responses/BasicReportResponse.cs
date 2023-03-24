namespace KfnApi.Models.Responses;

public sealed record BasicReportResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public required DateTime CreatedAt { get; set; }
}
