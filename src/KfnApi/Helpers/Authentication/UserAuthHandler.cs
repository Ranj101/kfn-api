using System.Security.Claims;
using System.Text.Encodings.Web;
using KfnApi.Abstractions;
using KfnApi.Errors.Exceptions;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace KfnApi.Helpers.Authentication;

public class UserAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService _userService;
    private readonly IAuthContext _authContext;

    public UserAuthHandler(
        UrlEncoder encoder,
        ILoggerFactory logger,
        IUserService userService,
        IAuthContext authContext,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _userService = userService;
        _authContext = authContext;
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var result = await HandleAuthenticateOnceSafeAsync();
        if (result.Failure is not AuthException e)
            return;

        Response.StatusCode = StatusCodes.Status403Forbidden;
        await Response.WriteAsJsonAsync(e.Message);
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.ContainsKey("Authorization"))
        {
            _authContext.SetAnonymous();

            var anonymousClaimsIdentity = new ClaimsIdentity(GenerateAnonymousClaims(), nameof(UserAuthHandler));
            var anonymousTicket = new AuthenticationTicket(new ClaimsPrincipal(anonymousClaimsIdentity), Scheme.Name);
            return AuthenticateResult.Success(anonymousTicket);
        }

        var identityUserId = Context.User.FindFirstValue(Constants.FirebaseUserClaimType.Id);

        if (string.IsNullOrEmpty(identityUserId))
            return AuthenticateResult.NoResult();

        var user = await _userService.GetByIdentityIdAsync(identityUserId) ?? await _userService.EnrollUserAsync(Context.GetFirebaseUser());

        if(user.State == UserState.Inactive)
            return AuthenticateResult.Fail(new AuthException("User is inactive."));

        _authContext.SetUser(user);

        var claimsIdentity = new ClaimsIdentity(GenerateClaims(user), nameof(UserAuthHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private static IEnumerable<Claim> GenerateClaims(in User user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        claims.AddRange(user.Roles.Select(role
            => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private static IEnumerable<Claim> GenerateAnonymousClaims()
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Role, Roles.Anonymous)
        };

        return claims;
    }
}
