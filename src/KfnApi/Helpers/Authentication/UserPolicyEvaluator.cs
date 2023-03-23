using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace KfnApi.Helpers.Authentication;

public class UserPolicyEvaluator : PolicyEvaluator
{
    public UserPolicyEvaluator(IAuthorizationService authorization) : base(authorization)
    {
    }

    public override async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        if (policy.AuthenticationSchemes != null && policy.AuthenticationSchemes.Count > 0)
        {
            ClaimsPrincipal? newPrincipal = null;
            DateTimeOffset? minExpiresUtc = null;
            foreach (var scheme in policy.AuthenticationSchemes)
            {
                var result = await context.AuthenticateAsync(scheme);
                if (result != null && result.Succeeded)
                {
                    newPrincipal = SecurityHelper.MergeUserPrincipal(newPrincipal, result.Principal);

                    if (minExpiresUtc is null || result.Properties?.ExpiresUtc < minExpiresUtc)
                    {
                        minExpiresUtc = result.Properties?.ExpiresUtc;
                    }
                } else if (result != null && result.Failure != null)
                {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity());
                    return AuthenticateResult.NoResult();
                }
            }

            if (newPrincipal != null)
            {
                context.User = newPrincipal;
                var ticket = new AuthenticationTicket(newPrincipal, string.Join(";", policy.AuthenticationSchemes));
                // ExpiresUtc is the easiest property to reason about when dealing with multiple schemes
                // SignalR will use this property to evaluate auth expiration for long running connections
                ticket.Properties.ExpiresUtc = minExpiresUtc;
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                context.User = new ClaimsPrincipal(new ClaimsIdentity());
                return AuthenticateResult.NoResult();
            }
        }

        // No modifications made to the HttpContext so let's use the existing result if it exists
        return context.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult ?? DefaultAuthenticateResult(context);

        static AuthenticateResult DefaultAuthenticateResult(HttpContext context)
        {
            return (context.User?.Identity?.IsAuthenticated ?? false)
                ? AuthenticateResult.Success(new AuthenticationTicket(context.User, "context.User"))
                : AuthenticateResult.NoResult();
        }
    }
}
