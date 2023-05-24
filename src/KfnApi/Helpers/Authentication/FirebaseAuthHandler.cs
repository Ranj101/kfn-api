using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace KfnApi.Helpers.Authentication;

public class FirebaseAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly FirebaseAuthenticationFunctionHandler _authenticationFunctionHandler;

    public FirebaseAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseAuthenticationFunctionHandler authenticationFunctionHandler)
        : base(options, logger, encoder, clock)
    {
        _authenticationFunctionHandler = authenticationFunctionHandler;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return _authenticationFunctionHandler.HandleAuthenticateAsync(Context);
    }
}
