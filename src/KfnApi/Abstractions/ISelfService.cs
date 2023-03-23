using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface ISelfService
{
    Task<User?> GetSelfAsync();
}
