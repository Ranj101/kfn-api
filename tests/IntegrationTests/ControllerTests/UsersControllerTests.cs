using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using IntegrationTests.Extensions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Models.Enums.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.ControllerTests;

public class UsersControllerTests : Base
{
    private const string BaseUrl = "v1/users";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public UsersControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetUserProfiles()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + "/profiles");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserProfileById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/profiles/{admin.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsers()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{admin.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserStateToInactive()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateUserStateRequest>()
            .With(x => x.Trigger, UserTrigger.Deactivate)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{admin.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserStateToActive()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        var customer = CreateCustomer();
        customer.State = UserState.Inactive;

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateUserStateRequest>()
            .With(x => x.Trigger, UserTrigger.Reactivate)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{customer.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
