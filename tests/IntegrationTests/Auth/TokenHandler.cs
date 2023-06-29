using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;
using IntegrationTests.Abstractions;
using IntegrationTests.Models;
using KfnApi;
using KfnApi.Models.Common;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Auth;

public class TokenHandler : ITokenHandler
{
    private readonly TokenSettings _tokenSettings;
    private readonly SigningConfigurations _signingConfig;
    private readonly Fixture _fixture = new();

    public TokenHandler(IOptions<TokenSettings> tokenSettings)
    {
        var signingConfig = new SigningConfigurations();
        signingConfig.SetAccessTokenSigningCredentials(tokenSettings.Value.AccessTokenSecret);

        _tokenSettings = tokenSettings.Value;
        _signingConfig = signingConfig;
    }

    public string GenerateAccessToken(string identityId, bool isNewUser)
    {
        var claims = new List<Claim> { new(Constants.FirebaseUserClaimType.Id, identityId) };

        if (isNewUser)
        {
            var firebaseUser = _fixture.Create<FirebaseUser>();

            claims.AddRange(new List<Claim>
            {
                new(Constants.FirebaseUserClaimType.Email, firebaseUser.Email),
                new(Constants.FirebaseUserClaimType.ProfilePicture, Constants.DefaultProductPicture.ToString()),
                new(Constants.FirebaseUserClaimType.Username, firebaseUser.FirstName + "_+_" + firebaseUser.LastName),
                new(Constants.FirebaseUserClaimType.EmailVerified, firebaseUser.EmailVerified.ToString())
            });
        }

        var securityToken = new JwtSecurityToken
        (
            issuer : _tokenSettings.Issuer,
            audience : _tokenSettings.Audience,
            claims: claims,
            expires : DateTime.UtcNow.AddMinutes(_tokenSettings.AccessTokenExpiration),
            notBefore : DateTime.UtcNow,
            signingCredentials : _signingConfig.AccessTokenSigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
