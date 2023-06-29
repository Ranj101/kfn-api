using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace KfnApi.Helpers.Authentication;

public class FirebaseAuthenticationFunctionHandler
{
    private const string BearerPrefix = "Bearer ";

    private readonly FirebaseApp _firebaseApp;

    public FirebaseAuthenticationFunctionHandler(FirebaseApp firebaseApp)
    {
        _firebaseApp = firebaseApp;
    }

    public Task<AuthenticateResult> HandleAuthenticateAsync(HttpRequest request) =>
        HandleAuthenticateAsync(request.HttpContext);

    public async Task<AuthenticateResult> HandleAuthenticateAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        string? bearerToken = context.Request.Headers["Authorization"];

        if (bearerToken == null || !bearerToken.StartsWith(BearerPrefix))
            return AuthenticateResult.Fail("Invalid scheme.");

        var token = bearerToken[BearerPrefix.Length..];

        try
        {
            var firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);
            return AuthenticateResult.Success(CreateAuthenticationTicket(firebaseToken));
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex);
        }
    }

    private static AuthenticationTicket CreateAuthenticationTicket(FirebaseToken firebaseToken)
    {
        var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>
        {
            new (ToClaims(firebaseToken.Claims), nameof(ClaimsIdentity))
        });

        return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
    }

    private static IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
    {
        return new List<Claim>
        {
            new (Constants.FirebaseUserClaimType.Id, claims.GetValueOrDefault("user_id", "").ToString()!),
            new (Constants.FirebaseUserClaimType.Username, claims.GetValueOrDefault("name", "").ToString()!),
            new (Constants.FirebaseUserClaimType.Email, claims.GetValueOrDefault("email", "").ToString()!),
            new (Constants.FirebaseUserClaimType.ProfilePicture, claims.GetValueOrDefault("picture", "").ToString()!),
            new (Constants.FirebaseUserClaimType.EmailVerified, claims.GetValueOrDefault("email_verified", "").ToString()!),
        };
    }
}
