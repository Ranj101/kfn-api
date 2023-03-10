using Microsoft.AspNetCore.Authorization;

namespace KfnApi.Helpers.Authorization.Policy;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(params Permission[] permissions)
    {
        var policy = string.Empty;

        foreach (var permission in permissions)
        {
            var policyAddition = $"{(int)permission}";

            if (string.IsNullOrEmpty(policy))
                policy = policyAddition;
            else
                policy += $";{policyAddition}";
        }

        Policy = policy;
    }

    public static HashSet<Permission> Parse(in string policy)
    {
        var permissions = new HashSet<Permission>();

        if (string.IsNullOrEmpty(policy))
            return permissions;

        var span = policy.AsSpan();

        while (true)
        {
            var nextPart = span.IndexOf(';');

            if (nextPart == -1)
            {
                permissions.Add((Permission)int.Parse(span));
                return permissions;
            }

            permissions.Add((Permission)int.Parse(span[..nextPart]));
            span = span[(nextPart + 1)..];
        }
    }
}
