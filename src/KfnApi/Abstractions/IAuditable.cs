namespace KfnApi.Abstractions;

public interface IAuditable
{
    Guid CreatedBy { get; init; }
    Guid? UpdatedBy { get; set; }
    DateTime CreatedAt { get; init; }
    DateTime? UpdatedAt { get; set; }
}
