﻿using System.ComponentModel.DataAnnotations;
using KfnApi.Abstractions;
using KfnApi.Models.Enums;

namespace KfnApi.Models.Requests;

public sealed record GetAllUsersRequest : IPaginatedRequest, ISortedRequest
{
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [Required]
    public SortBy SortBy { get; set; }

    [Required]
    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; }

    public string? SearchByEmail { get; set; }
}
