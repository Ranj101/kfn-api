using System.Net;
using IdentityModel.Client;
using KfnApi.Models.Settings;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

namespace KfnApi.Helpers.Authentication;

public class MachineToMachineBearerTokenHandler : DelegatingHandler
{
    private readonly AuthOptions _auth;
    private readonly IFusionCache _cache;
    private readonly HttpClient _identityClient;

    private const string M2MTokenKey = "kurdistan.food.network-api.m2m-access-token";

    public MachineToMachineBearerTokenHandler(IHttpClientFactory clientFactory, IOptions<AuthOptions> auth, IFusionCache cache)
    {
        _cache = cache;
        _auth = auth.Value;
        _identityClient = clientFactory.CreateClient(AuthDefaults.IdentityClient);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        if (!string.IsNullOrEmpty(accessToken))
            request.SetBearerToken(accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            await _cache.RemoveAsync(M2MTokenKey, token: cancellationToken);

        return response;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var accessToken = await _cache.TryGetAsync<string>(M2MTokenKey, token: cancellationToken);

        if (accessToken.GetValueOrDefault() is not null)
            return accessToken.Value;

        var tokenEndPoint = (await _identityClient.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken)).TokenEndpoint;

        var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
        {
            Address = tokenEndPoint,
            ClientId = _auth.M2MClientId,
            ClientSecret = _auth.M2MClientSecret,
            Parameters = new Parameters
            {
                {"audience", _auth.WrapperUrl}
            }
        };

        var response = await _identityClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest, cancellationToken);

        if (response.IsError)
        {
            await _cache.RemoveAsync(M2MTokenKey, token: cancellationToken);
            throw response.Exception ?? new HttpRequestException(response.Error, inner: response.Exception, statusCode: response.HttpStatusCode);
        }

        var expiresInTime = TimeSpan.FromSeconds(response.ExpiresIn);
        // if (expiresInTime > TimeSpan.FromMinutes(6))
        //     expiresInTime -= TimeSpan.FromMinutes(5);

        await _cache.SetAsync(
            M2MTokenKey,
            response.AccessToken,
            options => options.SetDuration(expiresInTime),
            token: cancellationToken
        );

        return response.AccessToken;
    }
}
