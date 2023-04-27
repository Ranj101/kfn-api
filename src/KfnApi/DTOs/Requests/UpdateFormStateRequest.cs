using System.ComponentModel.DataAnnotations;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record UpdateFormStateRequest
{
    [Required]
    [EnumDataType(typeof(ApprovalFormTrigger))]
    public ApprovalFormTrigger Trigger { get; set; }
}
