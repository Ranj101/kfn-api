namespace KfnApi.DTOs.Responses;

public class UploadResponse
{
    public required Guid Key { get; set; }
    public required string OriginalName { get; set; }
    public required string ContentType { get; set; }
    public required long Size { get; set; }
    public DateTime DateUploaded { get; set; }
}
