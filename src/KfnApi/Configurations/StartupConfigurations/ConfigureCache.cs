using System.Text.Json;
using KfnApi.Models.Settings;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureCache(this IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(RedisOptions.SectionName);

        services.AddOptions<RedisOptions>()
                .Bind(configuration)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        var options = configuration.Get<RedisOptions>();

        services.AddFusionCache(ConfigureFusionCache)
                .AddFusionCacheSystemTextJsonSerializer(ConfigureFusionCacheJsonSerializer());

        UseRedis();

        return services;

        void ConfigureFusionCache(FusionCacheOptions opts)
        {
            opts.DefaultEntryOptions = new() { Duration = TimeSpan.FromMinutes(5) };
        }

        void UseRedis()
        {
            services.AddStackExchangeRedisCache(ConfigureRedisCache);

            void ConfigureRedisCache(RedisCacheOptions opts)
            {
                opts.Configuration = options!.ConnectionString;
            }
        }

        JsonSerializerOptions ConfigureFusionCacheJsonSerializer()
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = Constants.SerializerOptions.DefaultIgnoreCondition
            };

            foreach (var converter in Constants.SerializerOptions.Converters)
            {
                jsonSerializerOptions.Converters.Add(converter);
            }

            return jsonSerializerOptions;
        }
    }
}
