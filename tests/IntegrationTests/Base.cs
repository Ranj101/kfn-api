using System.Net.Http.Headers;
using AutoFixture;
using IntegrationTests.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class Base : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory _factory;
    private readonly Fixture _fixture = new();

    protected Base(TestApplicationFactory factory)
    {
        _factory = factory;
    }

    protected HttpClient GetClient(string identityId, bool isNewUser = false, bool withoutHeader = false)
    {
        var httpClient = _factory.CreateClient();

        if (withoutHeader)
            return httpClient;

        var tokenHandler = _factory.Services.GetRequiredService<ITokenHandler>();
        var accessToken = tokenHandler.GenerateAccessToken(identityId, isNewUser);

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
            .With(x => x.State, UserState.Active)
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
            .With(x => x.State, UserState.Active)
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
            .With(x => x.State, UserState.Active)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.ApprovalForms)
            .Create();
    }
}
