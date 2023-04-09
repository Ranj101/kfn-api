using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record GetAllUsersRequest : IPaginatedRequest, ISortedRequest<SortUserBy>
{
    public GetAllUsersRequest() { }
    public GetAllUsersRequest(GetAllProfilesRequest request)
    {
        PageIndex = request.PageIndex;
        PageSize = request.PageSize;
        SortBy = request.SortBy;
        SortDirection = request.SortDirection;
        FilterByEmail = request.FilterByEmail;
        FilterByState = UserState.Active;
    }

    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [Required]
    public SortUserBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public UserState? FilterByState { get; set; }

    public string? FilterByEmail { get; set; }
}
