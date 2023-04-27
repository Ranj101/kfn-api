using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record GetAllOrdersRequest : IPaginatedRequest, ISortedRequest<SortOrderBy>
{
    public GetAllOrdersRequest() { }

    public GetAllOrdersRequest(GetAllBasicOrdersRequest request)
    {
        PageIndex = request.PageIndex;
        PageSize = request.PageSize;
        SortBy = request.SortBy;
        SortDirection = request.SortDirection;
        FilterByState = request.FilterByState;
        FilterByProducerId = request.FilterByProducerId;
    }

    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [Required]
    public SortOrderBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public Guid? FilterByProducerId { get; set; }

    public OrderState? FilterByState { get; set; }

    public string? FilterByUserEmail { get; set; }

    public Guid? FilterByUserId { get; set; }
}
