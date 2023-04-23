using System.ComponentModel.DataAnnotations;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public sealed record UpdateProductStateRequest
{
    [Required]
    [EnumDataType(typeof(ExposedProductTrigger))]
    public ExposedProductTrigger Trigger { get; set; }
}
