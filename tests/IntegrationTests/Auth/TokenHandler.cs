using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IntegrationTests.Abstractions;
using IntegrationTests.Models;
using KfnApi;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Auth;

public class TokenHandler : ITokenHandler
{
    private readonly TokenSettings _tokenSettings;
    private readonly SigningConfigurations _signingConfig;

    public TokenHandler(IOptions<TokenSettings> tokenSettings)
    {
        var signingConfig = new SigningConfigurations();
        signingConfig.SetAccessTokenSigningCredentials(tokenSettings.Value.AccessTokenSecret);

        _tokenSettings = tokenSettings.Value;
        _signingConfig = signingConfig;
    }

    public string GenerateAccessToken(string identityId)
    {
        var securityToken = new JwtSecurityToken
        (
            issuer : _tokenSettings.Issuer,
            audience : _tokenSettings.Audience,
            claims: new []{ new Claim(Constants.FirebaseUserClaimType.Id, identityId) },
            expires : DateTime.UtcNow.AddMinutes(_tokenSettings.AccessTokenExpiration),
            notBefore : DateTime.UtcNow,
            signingCredentials : _signingConfig.AccessTokenSigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
