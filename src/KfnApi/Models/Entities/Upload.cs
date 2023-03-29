
using System.ComponentModel.DataAnnotations;

namespace KfnApi.Models.Entities;

public sealed record Upload
{
    [Key]
    public required Guid Key { get; set; }

    public required string OriginalName { get; set; }
    public required string ContentType { get; set; }
    public required long Size { get; set; }
    public DateTime DateUploaded { get; set; }

    // Database Relations
    public List<User>? Users { get; set; }
    public List<Producer>? Producers { get; set; }
    public List<Product>? Products { get; set; }
    public List<ApprovalForm>? ApprovalForms { get; set; }
}
