using KfnApi.Models.Common;

namespace KfnApi.Abstractions;

public interface IRemoteUserService
{
    Task<AuthUser?> GetByIdAsync(string id, CancellationToken ct = default);
}
