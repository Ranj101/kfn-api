using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;

namespace KfnApi.Services;

public class PermissionService : IPermissionService
{
    private readonly IAuthContext _authContext;

    public PermissionService(IAuthContext authContext)
    {
        _authContext = authContext;
    }

    public bool HasPermission(Permission permissionToAccess)
    {
        if (!_authContext.HasUser() && !_authContext.IsAnonymous())
            return false;

        if (permissionToAccess is Permission.None)
            return true;

        var currentPermissions = _authContext.GetPermissions();

        return currentPermissions.Contains(permissionToAccess);
    }

    public bool HasAnyPermission(HashSet<Permission> permissionsToAccess)
    {
        if (!_authContext.HasUser() && !_authContext.IsAnonymous())
            return false;

        if (permissionsToAccess.Count == 1  && permissionsToAccess.First() is Permission.None)
            return true;

        var currentPermissions = _authContext.GetPermissions();

        var hasRequiredPermission = currentPermissions.Any(currentPermission => permissionsToAccess
            .Any(permissionToAccess => permissionToAccess == currentPermission));

        return hasRequiredPermission;
    }
}
