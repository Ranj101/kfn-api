using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Responses;

public sealed record OrderListResponse
{
    public required Guid Id { get; set; }
    public required double TotalPrice { get; set; }
    public required string Location { get; set; }
    public required OrderState State { get; set; }
}
