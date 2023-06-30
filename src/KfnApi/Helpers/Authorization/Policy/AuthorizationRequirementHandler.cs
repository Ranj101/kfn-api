using KfnApi.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace KfnApi.Helpers.Authorization.Policy;

public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly IAuthContext _authContext;
    private readonly IPermissionService _permissionService;

    public AuthorizationRequirementHandler(IPermissionService permissionService, IAuthContext authContext)
    {
        _permissionService = permissionService;
        _authContext = authContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        if (requirement.Permissions.Any())
            HandlePermissionRequirement(context, requirement);

        return Task.CompletedTask;
    }

    private void HandlePermissionRequirement(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        var permissions = requirement.Permissions;

        if (!_authContext.HasUser() && !_authContext.IsAnonymous())
        {
            context.Fail();
            return;
        }

        if (_permissionService.HasAnyPermission(permissions))
            context.Succeed(requirement);
    }
}
