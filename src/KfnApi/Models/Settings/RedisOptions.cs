using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Settings;

public class RedisOptions
{
    public const string SectionName = "Redis";

    [Required]
    public required string ConnectionString { get; set; }
}
