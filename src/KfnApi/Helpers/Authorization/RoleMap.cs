using KfnApi.Abstractions;

namespace KfnApi.Helpers.Authorization;

public class RoleMap : IRoleMap
{
    private readonly IReadOnlyDictionary<string, Permission[]> _roleDefinitions;

    public RoleMap()
    {
        _roleDefinitions = CreateRolePermissionsMap();
    }

    public IReadOnlyDictionary<string, Permission[]> GetRoleDefinitions()
        => _roleDefinitions;

    private static IReadOnlyDictionary<string, Permission[]> CreateRolePermissionsMap()
    {
        // Skip the `none` permission
        var allPermissions = Enum.GetValues<Permission>()[1..].ToArray();

        return new Dictionary<string, Permission[]>
        {
            { Roles.SuperAdmin, allPermissions },
            { Roles.SystemAdmin, allPermissions }
        };
    }
}
