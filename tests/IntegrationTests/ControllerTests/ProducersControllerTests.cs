using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using IntegrationTests.Extensions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.ControllerTests;

public class ProducersControllerTests : Base
{
    private const string BaseUrl = "v1/producers";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public ProducersControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetPages()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var producer = _fixture
            .Build<Producer>()
            .With(x => x.UserId, admin.Id)
            .With(x => x.State, ProducerState.Active)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.Products)
            .Create();

        _dbContext.Producers.Add(producer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + "/pages");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPageById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var producer = _fixture
            .Build<Producer>()
            .With(x => x.UserId, admin.Id)
            .With(x => x.State, ProducerState.Active)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.Products)
            .Create();

        _dbContext.Producers.Add(producer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/pages/{producer.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducers()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var producer = _fixture
            .Build<Producer>()
            .With(x => x.UserId, admin.Id)
            .With(x => x.State, ProducerState.Active)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.Products)
            .Create();

        _dbContext.Producers.Add(producer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducerById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var producer = _fixture
            .Build<Producer>()
            .With(x => x.UserId, admin.Id)
            .With(x => x.State, ProducerState.Active)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.Products)
            .Create();

        _dbContext.Producers.Add(producer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{producer.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProducerStateToInactive()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var producerUser = CreateProducer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(producerUser);
        await _dbContext.SaveChangesAsync();

        var producer = _fixture
            .Build<Producer>()
            .With(x => x.UserId, producerUser.Id)
            .With(x => x.State, ProducerState.Active)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.Products)
            .Create();

        _dbContext.Producers.Add(producer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateProducerStateRequest>()
            .With(x => x.Trigger, ProducerTrigger.Deactivate)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{producer.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProducerStateToActive()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var producer = _fixture
            .Build<Producer>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.State, ProducerState.Inactive)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.AbuseReports)
            .Without(x => x.Products)
            .Create();

        _dbContext.Producers.Add(producer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateProducerStateRequest>()
            .With(x => x.Trigger, ProducerTrigger.Reactivate)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{producer.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
