using System.ComponentModel.DataAnnotations;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Models.Requests;

public class UpdateUserStateRequest
{
    [Required]
    [EnumDataType(typeof(UserTrigger))]
    public UserTrigger Trigger { get; set; }
}
