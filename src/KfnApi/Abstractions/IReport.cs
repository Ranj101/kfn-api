namespace KfnApi.Abstractions;

public interface IReport : IAuditable
{
    Guid Id { get; set; }
    string Title { get; set; }
    string Summary { get; set; }
}
