using System.Security.Claims;
using System.Text.Encodings.Web;
using KfnApi.Errors.Exceptions;
using KfnApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace KfnApi.Helpers.Authentication;

public class UserHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UserService _userService;

    public UserHandler(
        UrlEncoder encoder,
        ILoggerFactory logger,
        UserService userService,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userSub = Context.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userSub))
            return AuthenticateResult.Fail(new AuthException("Name identifier header is missing."));

        var user = await _userService.GetByIdAsync(userSub) ?? await _userService.EnrollUserAsync(userSub);

        if(user is null)
            return AuthenticateResult.Fail(new AuthException("User identity not found."));

        await _userService.UpsertCacheAsync(user);

        throw new NotImplementedException();

        return await base.AuthenticateAsync();
    }
}
