using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using KfnApi.Helpers.Authentication;
using KfnApi.Models.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(FirebaseOptions.SectionName);

        services
            .AddOptions<FirebaseOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var firebaseOptions = configuration.Get<FirebaseOptions>();

        services.AddSingleton(FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJsonParameters(new JsonCredentialParameters
            {
                Type = firebaseOptions!.Type,
                ProjectId = firebaseOptions.ProjectId,
                PrivateKeyId = firebaseOptions.PrivateKeyId,
                PrivateKey = firebaseOptions.PrivateKey,
                ClientEmail = firebaseOptions.ClientEmail,
                ClientId = firebaseOptions.ClientId,
                TokenUrl = firebaseOptions.TokenUri
            })
        }));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, FirebaseAuthHandler>(JwtBearerDefaults.AuthenticationScheme, _ => {})
            .AddScheme<AuthenticationSchemeOptions, UserAuthHandler>(Constants.AuthScheme, _ => {});

        services.AddScoped<FirebaseAuthenticationFunctionHandler>();

        //TODO: Remove this later
        IdentityModelEventSource.ShowPII = true;

        return services;
    }
}
