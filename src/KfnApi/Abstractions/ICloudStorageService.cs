namespace KfnApi.Abstractions;

public interface ICloudStorageService
{
    string GetPreSignedUrl(string key);
    Task PutObjectAsync(string key, IFormFile file, CancellationToken token);
    Task DeleteObjectAsync(string key, CancellationToken token);
    Task DeleteObjectAsync(string key);
}
