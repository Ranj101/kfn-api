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

public class ReportsControllerTests : Base
{
    private const string BaseUrl = "v1/report";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public ReportsControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetReports()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

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

        var userReport = _fixture
            .Build<UserReport>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Create();

        _dbContext.UserReports.Add(userReport);
        await _dbContext.SaveChangesAsync();

        var producerReport = _fixture
            .Build<ProducerReport>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Create();

        _dbContext.ProducerReports.Add(producerReport);
        await _dbContext.SaveChangesAsync();

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserReportById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var userReport = _fixture
            .Build<UserReport>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Create();

        _dbContext.UserReports.Add(userReport);
        await _dbContext.SaveChangesAsync();

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{userReport.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducerReportById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

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

        var producerReport = _fixture
            .Build<ProducerReport>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Create();

        _dbContext.ProducerReports.Add(producerReport);
        await _dbContext.SaveChangesAsync();

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{producerReport.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SubmitUserReport()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var request = _fixture.Create<SubmitReportRequest>();

        // ~~~ Act ~~~
        var httpResponse = await client.PostAsJsonAsync(BaseUrl + $"/{customer.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task SubmitProducerReport()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

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

        var request = _fixture.Create<SubmitReportRequest>();

        // ~~~ Act ~~~
        var httpResponse = await client.PostAsJsonAsync(BaseUrl + $"/{producer.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
