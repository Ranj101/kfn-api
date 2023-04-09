using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record GetAllProducersRequest : IPaginatedRequest, ISortedRequest<SortProducerBy>
{
    public GetAllProducersRequest() { }
    public GetAllProducersRequest(GetAllProducerPagesRequest request)
    {
        PageIndex = request.PageIndex;
        PageSize = request.PageSize;
        SortBy = request.SortBy;
        SortDirection = request.SortDirection;
        FilterByUserEmail = request.FilterByUserEmail;
        FilterByUserId = request.FilterByUserId;
        FilterByState = ProducerState.Active;
    }

    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)] public int PageSize { get; set; } = 10;

    [Required]
    public SortProducerBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public ProducerState? FilterByState { get; set; }

    public string? FilterByUserEmail { get; set; }

    public Guid? FilterByUserId { get; set; }
}
