using System.Text.Json;
using System.Text.Json.Serialization;

namespace KfnApi;

public static class Constants
{
    public const string AuthScheme = "userAuthScheme";

    public static Guid DefaultProductPicture { get; } = new("b9779efb-675b-4d5d-9a5a-885f56f992c1");

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
