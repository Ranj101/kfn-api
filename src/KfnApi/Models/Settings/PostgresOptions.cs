using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Settings;

public sealed record PostgresOptions
{
    public const string SectionName = "Postgres";

    [Required]
    public required string ConnectionString { get; set; }
}
