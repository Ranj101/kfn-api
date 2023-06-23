using System.Net.Http.Headers;
using AutoFixture;
using IntegrationTests.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class Base : IClassFixture<TestApplicationFactory>, IAsyncLifetime
{
    private readonly TestApplicationFactory _factory;
    private readonly Fixture _fixture = new();

    protected Base(TestApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        return dbContext.Database.EnsureDeletedAsync();
    }

    protected HttpClient GetClient(string identityId)
    {
        var httpClient = _factory.CreateClient();

        var tokenHandler = _factory.Services.GetRequiredService<ITokenHandler>();
        var accessToken = tokenHandler.GenerateAccessToken(identityId);

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        return httpClient;
    }

    protected User CreateAdmin()
    {
        return _fixture
            .Build<User>()
            .With(x => x.Roles, new List<string> { Roles.SuperAdmin, Roles.SystemAdmin })
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.ApprovalForms)
            .Create();
    }

    protected User CreateCustomer()
    {
        return _fixture
            .Build<User>()
            .With(x => x.Roles, new List<string> { Roles.Customer })
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.ApprovalForms)
            .Create();
    }

    protected User CreateProducer()
    {
        return _fixture
            .Build<User>()
            .With(x => x.Roles, new List<string> { Roles.Producer })
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.ApprovalForms)
            .Create();
    }
}
