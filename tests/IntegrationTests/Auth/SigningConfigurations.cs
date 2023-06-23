using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Auth;

public class SigningConfigurations
{
    public SecurityKey? SecurityKey { get; private set; }
    public SigningCredentials? AccessTokenSigningCredentials { get; private set; }

    public void SetAccessTokenSigningCredentials(string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        SecurityKey = new SymmetricSecurityKey(keyBytes);
        AccessTokenSigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha512Signature);
    }

    public SecurityKey GetTokenSecurityKey(string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        SecurityKey = new SymmetricSecurityKey(keyBytes);
        return SecurityKey;
    }
}
