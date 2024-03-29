﻿using System.ComponentModel.DataAnnotations;
using KfnApi.Models.Common;

namespace KfnApi.DTOs.Requests;

public sealed record SubmitFormRequest
{
    [Required]
    [MaxLength(350)]
    public required string ProducerName { get; set; }

    [Required]
    public required List<string> Locations { get; set; }

    [Required]
    public required Time OpeningTime { get; set; }

    [Required]
    public required Time ClosingTime { get; set; }

    [Required]
    [MinLength(1), MaxLength(3)]
    public required List<Guid> Uploads { get; set; }
}
