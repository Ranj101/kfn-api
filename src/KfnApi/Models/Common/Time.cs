using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Common;

public class Time
{
    [Range(0, 23)]
    public int Hour { get; set; }

    [Range(0, 59)]
    public int Minute { get; set; }
}
