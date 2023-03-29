using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using KfnApi.Models.Settings;

namespace KfnApi.Configurations.StartupConfigurations;

public static partial class StartupConfigurations
{
    public static IServiceCollection ConfigureCloudStorage(this IServiceCollection services, IConfiguration configuration)
    {
        configuration = configuration.GetRequiredSection(CloudStorageOptions.SectionName);

        services.AddOptions<CloudStorageOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var options = configuration.Get<CloudStorageOptions>();

        var cloudConfig = GoogleCredential.FromJsonParameters(new JsonCredentialParameters
        {
            Type = options!.Type,
            ProjectId = options.ProjectId,
            PrivateKeyId = options.PrivateKeyId,
            PrivateKey = options.PrivateKey,
            ClientEmail = options.ClientEmail,
            ClientId = options.ClientId,
            TokenUrl = options.TokenUri
        });

        var client = StorageClient.Create(cloudConfig);
        var signer = UrlSigner.FromCredential(cloudConfig);

        services.AddSingleton(client);
        services.AddSingleton(signer);

        return services;
    }
}
