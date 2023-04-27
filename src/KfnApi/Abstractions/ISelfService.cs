using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface ISelfService
{
    Task<User?> GetSelfAsync();
    Task<Result<User>> UpdateSelfAsync(UpdateSelfRequest request);
}
