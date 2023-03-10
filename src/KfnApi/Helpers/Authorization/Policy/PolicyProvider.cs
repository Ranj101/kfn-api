using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KfnApi.Helpers.Authorization.Policy;

public class PolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public PolicyProvider(IOptions<AuthorizationOptions> options, IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var permission = RequirePermissionAttribute.Parse(policyName);

        if (permission.IsNullOrEmpty())
            return await _fallbackPolicyProvider.GetPolicyAsync(policyName);

        var allSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        var policy = new AuthorizationPolicyBuilder(allSchemes.Select(s => s.Name).ToArray());

        policy.AddRequirements(new AuthorizationRequirement(permission));

        return policy.Build();
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy?>(null);
    }
}
