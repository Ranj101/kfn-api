using Microsoft.AspNetCore.Authorization;

namespace KfnApi.Helpers.Authorization.Policy;

public class AuthorizationRequirement : IAuthorizationRequirement
{
    public HashSet<Permission> Permissions { get; }

    public AuthorizationRequirement(HashSet<Permission> permission)
    {
        Permissions = permission;
    }
}
