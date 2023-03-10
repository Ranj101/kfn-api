using System.Security.Claims;
using System.Text.Encodings.Web;
using KfnApi.Abstractions;
using KfnApi.Errors.Exceptions;
using KfnApi.Models.Entities;
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
        var userSub = Context.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userSub))
            return AuthenticateResult.Fail(new AuthException("Name identifier header is missing."));

        var user = await _userService.GetByIdAsync(userSub) ?? await _userService.EnrollUserAsync(userSub);

        if(user is null)
            return AuthenticateResult.Fail(new AuthException("User identity not found."));

        _authContext.SetUser(user);

        var claimsIdentity = new ClaimsIdentity(GenerateClaims(user), nameof(UserAuthHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private static IEnumerable<Claim> GenerateClaims(in User user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id)
        };

        claims.AddRange(user.Roles.Select(role
            => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
}
