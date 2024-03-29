﻿using System.ComponentModel.DataAnnotations;

namespace KfnApi.DTOs.Requests;

public sealed record SubmitReportRequest
{
    [Required]
    [MaxLength(350)]
    public required string Title { get; set; }

    [Required]
    [MaxLength(int.MaxValue)]
    public required string Summary { get; set; }
}
