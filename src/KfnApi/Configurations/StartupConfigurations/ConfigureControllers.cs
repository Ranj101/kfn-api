using KfnApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers(opts =>
            {
                opts.Filters.Add<UnhandledExceptionFilter>();
            })
            .ConfigureApiBehaviorOptions(opts =>
            {
                opts.InvalidModelStateResponseFactory = InvalidModelState;
            });

        services.ConfigureOptions<ConfigureJsonOptions>();

        return services;

        static IActionResult InvalidModelState(ActionContext context)
        {
            var errors = context.ModelState.SelectMany(v => v.Value!.Errors
                .Select(ToError)).ToList();

            return new BadRequestObjectResult(errors);

            static object ToError(ModelError error)
            {
                return new
                {
                    Title = "Validation Error",
                    Detail = error.ErrorMessage
                };
            }
        }
    }

    private sealed class ConfigureJsonOptions : IConfigureOptions<JsonOptions>
    {
        public void Configure(JsonOptions opts)
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = Constants.SerializerOptions.PropertyNamingPolicy;
            opts.JsonSerializerOptions.DefaultIgnoreCondition = Constants.SerializerOptions.DefaultIgnoreCondition;
            foreach (var converter in Constants.SerializerOptions.Converters)
                opts.JsonSerializerOptions.Converters.Add(converter);
        }
    }
}
