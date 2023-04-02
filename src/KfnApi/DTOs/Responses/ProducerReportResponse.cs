namespace KfnApi.DTOs.Responses;

public sealed record ProducerReportResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }

    public required Guid CreatedBy { get; init; }
    public required DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public required ProducerPageResponse Producer { get; set; }
}
