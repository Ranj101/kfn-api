using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;

namespace KfnApi.DTOs.Requests;

public sealed record GetAllProducerPagesRequest : IPaginatedRequest, ISortedRequest<SortProducerBy>
{
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)] public int PageSize { get; set; } = 10;

    [Required]
    public SortProducerBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public string? FilterByUserEmail { get; set; }

    public Guid? FilterByUserId { get; set; }
}
