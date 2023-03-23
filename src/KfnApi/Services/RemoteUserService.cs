using System.Text.Json;
using KfnApi.Abstractions;
using KfnApi.Models.Common;
using KfnApi.Models.Settings;

namespace KfnApi.Services;

public class RemoteUserService : IRemoteUserService
{
    private readonly HttpClient _httpClient;

    private const string GetByIdEndpoint = "users/{0}";
    private const string GetRoleByIdEndpoint = "users/{0}/roles";
    public RemoteUserService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(AuthDefaults.IdentityWrapperClient);
    }

    public async Task<AuthUser?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        using var userResponse = await _httpClient.GetAsync(string.Format(GetByIdEndpoint, id), ct);
        var authUser = await DeserializeJsonAsync<AuthUser>(await userResponse.Content.ReadAsStreamAsync(ct), ct);

        using var roleResponse = await _httpClient.GetAsync(string.Format(GetRoleByIdEndpoint, id), ct);
        var userRoles = await DeserializeJsonAsync<List<AuthUserRole>>(await roleResponse.Content.ReadAsStreamAsync(ct), ct);

        if (authUser != null)
            authUser.Roles = userRoles?.Select(r => r.Name).ToList() ?? new ();

        return authUser;
    }

    private static Task<T?> DeserializeJsonAsync<T>(Stream stream, CancellationToken ct)
    {
        return JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct)
            .AsTask();
    }
}
