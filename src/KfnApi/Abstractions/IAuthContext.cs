using KfnApi.Helpers.Authorization;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IAuthContext
{
    bool HasUser();
    string GetUserId();
    User GetUser();
    bool IsAdmin();
    bool IsSuperAdmin();
    bool IsSystemAdmin();
    HashSet<Permission> GetPermissions();
    void SetUser(User user);
}
