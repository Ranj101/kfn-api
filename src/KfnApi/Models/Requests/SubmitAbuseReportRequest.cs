using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Requests;

public class SubmitAbuseReportRequest
{
    [Required]
    [MaxLength(350)]
    public required string Title { get; set; }

    [Required]
    [MaxLength(int.MaxValue)]
    public required string Summary { get; set; }
}
