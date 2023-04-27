using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IUploadService
{
    Task<Upload> UploadAsync(IFormFile file, CancellationToken token);
    Task<Result<Upload>> DeleteAsync(Guid key, CancellationToken token);
}
