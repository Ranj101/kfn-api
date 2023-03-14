using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;

namespace KfnApi.Models.Requests;

public class GetAllUsersRequest : IPaginatedRequest, ISortedRequest
{
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [Required]
    public SortBy SortBy { get; set; }

    [Required]
    public SortDirection SortDirection { get; set; }

    public string? SearchByEmail { get; set; }
}
