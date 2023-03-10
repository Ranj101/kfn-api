using System.Text.Json;
using KfnApi.Abstractions;
using KfnApi.Models.Common;
using KfnApi.Models.Settings;

namespace KfnApi.Services;

public class RemoteUserService : IRemoteUserService
{
    private readonly HttpClient _httpClient;

    private const string GetByIdEndpoint = "users/{0}";

    public RemoteUserService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(AuthDefaults.IdentityWrapperClient);
    }

    public async Task<AuthUser?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync(string.Format(GetByIdEndpoint, id), ct);
        return await DeserializeJsonAsync(await response.Content.ReadAsStreamAsync(ct), ct);
    }

    private static Task<AuthUser?> DeserializeJsonAsync(Stream stream, CancellationToken ct)
    {
        return JsonSerializer.DeserializeAsync<AuthUser>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct)
            .AsTask();
    }
}
