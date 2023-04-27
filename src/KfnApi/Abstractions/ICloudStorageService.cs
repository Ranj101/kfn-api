namespace KfnApi.Abstractions;

public interface ICloudStorageService
{
    string GetPreSignedUrl(Guid key);
    Task PutObjectAsync(string key, IFormFile file, CancellationToken token);
    Task DeleteObjectAsync(string key, CancellationToken token);
    Task DeleteObjectAsync(string key);
}
