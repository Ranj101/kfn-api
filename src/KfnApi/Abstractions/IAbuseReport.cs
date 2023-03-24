namespace KfnApi.Abstractions;

public interface IAbuseReport : IAuditable
{
    Guid Id { get; set; }
    string Title { get; set; }
    string Summary { get; set; }
}
