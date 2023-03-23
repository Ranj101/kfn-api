using KfnApi.Helpers.Authorization;

namespace KfnApi.Abstractions;

public interface IRoleMap
{
    IReadOnlyDictionary<string, Permission[]> GetRoleDefinitions();
}
