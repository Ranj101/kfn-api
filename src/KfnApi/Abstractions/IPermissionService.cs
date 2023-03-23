using KfnApi.Helpers.Authorization;

namespace KfnApi.Abstractions;

public interface IPermissionService
{
    bool HasPermission(Permission permissionToAccess);
    bool HasAnyPermission(HashSet<Permission> permissionsToAccess);
}
