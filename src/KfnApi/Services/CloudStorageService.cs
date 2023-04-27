using Google.Cloud.Storage.V1;
using KfnApi.Abstractions;
using KfnApi.Models.Settings;
using Microsoft.Extensions.Options;

namespace KfnApi.Services;

public class CloudStorageService : ICloudStorageService
{
    private readonly UrlSigner _urlSigner;
    private readonly StorageClient _client;
    private readonly CloudStorageOptions _options;

    public CloudStorageService(StorageClient client, UrlSigner urlSigner, IOptions<CloudStorageOptions> options)
    {
        _client = client;
        _urlSigner = urlSigner;
        _options = options.Value;
    }

    public string GetPreSignedUrl(Guid key)
    {
        return _urlSigner.Sign(_options.Bucket, key.ToString(), TimeSpan.FromDays(1));
    }

    public async Task PutObjectAsync(string key, IFormFile file, CancellationToken token)
    {
        await _client.UploadObjectAsync(_options.Bucket, key, file.ContentType,
            file.OpenReadStream(), cancellationToken: token);
    }

    public async Task DeleteObjectAsync(string key, CancellationToken token)
    {
        await _client.DeleteObjectAsync(_options.Bucket, key, cancellationToken: token);
    }

    public async Task DeleteObjectAsync(string key)
    {
        await _client.DeleteObjectAsync(_options.Bucket, key);
    }
}
