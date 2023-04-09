using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record GetAllFormsRequest : IPaginatedRequest, ISortedRequest<SortFormBy>
{
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)] public int PageSize { get; set; } = 10;

    [Required]
    public SortFormBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public ApprovalFormState? FilterByState { get; set; }

    public string? FilterByUserEmail { get; set; }

    public Guid? FilterByUserId { get; set; }
}
