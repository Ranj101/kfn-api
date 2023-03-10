using System.Text.Json;
using System.Text.Json.Serialization;

namespace KfnApi;

public static class Constants
{
    public const string AuthScheme = "userAuthScheme";

    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}
