using System.ComponentModel.DataAnnotations;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.DTOs.Requests;

public class UpdateProducerStateRequest
{
    [Required]
    [EnumDataType(typeof(ProducerTrigger))]
    public ProducerTrigger Trigger { get; set; }
}
