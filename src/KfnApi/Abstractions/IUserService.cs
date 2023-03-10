using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> EnrollUserAsync(string id);
    Task UpsertCacheAsync(User user);
}
