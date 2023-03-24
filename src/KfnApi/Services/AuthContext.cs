using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Models.Entities;

namespace KfnApi.Services;

public class AuthContext : IAuthContext
{
    private readonly IRoleMap _roleMap;

    private User? _user;
    private bool _isSuperAdmin;
    private bool _isSystemAdmin;
    private HashSet<Permission> _permissions = new();

    public AuthContext(IRoleMap roleMap)
    {
        _roleMap = roleMap;
    }

    public bool HasUser()
        => _user is not null;

    public User GetUser()
        => _user!;

    public Guid GetUserId()
        => _user!.Id;

    public bool IsAdmin()
        => _isSuperAdmin || _isSystemAdmin;

    public bool IsSuperAdmin()
        => _isSuperAdmin;

    public bool IsSystemAdmin()
        => _isSystemAdmin;

    public HashSet<Permission> GetPermissions()
        => _permissions;

    public void SetUser(User user)
    {
        _user = user;

        _isSuperAdmin = user.Roles.Contains(Roles.SuperAdmin);
        _isSystemAdmin = user.Roles.Contains(Roles.SystemAdmin);

        _permissions = _roleMap.GetRoleDefinitions()
            .Where(kvp => user.Roles.Contains(kvp.Key))
            .SelectMany(kvp => kvp.Value)
            .ToHashSet();
    }
}
