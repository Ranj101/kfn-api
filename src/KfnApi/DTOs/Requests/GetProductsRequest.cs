using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;

namespace KfnApi.DTOs.Requests;

public class GetProductsRequest : IPaginatedRequest, ISortedRequest<SortProductBy>
{
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [Required]
    public SortProductBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public string? FilterByName { get; set; }

    public Guid? FilterByProducer { get; set; }
}
