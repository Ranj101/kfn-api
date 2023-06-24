namespace IntegrationTests.Abstractions;

public interface ITokenHandler
{
    string GenerateAccessToken(string identityId, bool isNewUser);
}
