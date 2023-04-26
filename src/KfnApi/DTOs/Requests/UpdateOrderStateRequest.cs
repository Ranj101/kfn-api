using System.ComponentModel.DataAnnotations;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record UpdateOrderStateRequest
{
    [Required]
    [EnumDataType(typeof(ExposedOrderTrigger))]
    public ExposedOrderTrigger Trigger { get; set; }
}
