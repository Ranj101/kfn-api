using KfnApi.Helpers.Authorization;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IAuthContext
{
    bool HasUser();
    User GetUser();
    Guid GetUserId();
    bool IsAdmin();
    bool IsProducer();
    bool IsSuperAdmin();
    bool IsSystemAdmin();
    HashSet<Permission> GetPermissions();
    void SetUser(User user);
}
