using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using IntegrationTests.Extensions;
using KfnApi.DTOs.Requests;
using KfnApi.DTOs.Responses;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.ControllerTests;

public class SelfControllerTests : Base
{
    private const string BaseUrl = "v1/self";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public SelfControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetSelfUserExists()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);
        var selfResponse = await httpResponse.Content.ReadFromJsonAsync<SelfResponse>();

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        selfResponse.Should().NotBeNull();
        selfResponse!.Roles.Should().Contain(x => x == Roles.SuperAdmin);
    }

    [Fact]
    public async Task GetSelfUserDoesNotExist()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var client = GetClient(admin.IdentityId, isNewUser: true);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);
        var selfResponse = await httpResponse.Content.ReadFromJsonAsync<SelfResponse>();

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        selfResponse.Should().NotBeNull();
        selfResponse!.Roles.Should().OnlyContain(x => x == Roles.Customer);
    }

    [Fact]
    public async Task UpdateSelf()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateSelfRequest>()
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl, request);
        var selfResponse = await httpResponse.Content.ReadFromJsonAsync<SelfResponse>();

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        selfResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateForm()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var form = _fixture
            .Build<ApprovalForm>()
            .With(x => x.UserId, admin.Id)
            .With(x => x.State, ApprovalFormState.Pending)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Uploads)
            .Create();

        _dbContext.ApprovalForms.Add(form);
        await _dbContext.SaveChangesAsync();

        var upload = _fixture
            .Build<Upload>()
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<SubmitFormRequest>()
            .With(x => x.Uploads, new List<Guid>{ upload.Key })
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + "/form", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateFormInvalidUpload()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var form = _fixture
            .Build<ApprovalForm>()
            .With(x => x.UserId, admin.Id)
            .With(x => x.State, ApprovalFormState.Pending)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Uploads)
            .Create();

        _dbContext.ApprovalForms.Add(form);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<SubmitFormRequest>()
            .With(x => x.Uploads, new List<Guid>{ Guid.NewGuid() })
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + "/form", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateFormNotFound()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var upload = _fixture
            .Build<Upload>()
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<SubmitFormRequest>()
            .With(x => x.Uploads, new List<Guid>{ upload.Key })
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + "/form", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProducer()
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

        var upload = _fixture
            .Build<Upload>()
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateProducerRequest>()
            .With(x => x.Uploads, new List<Guid>{ upload.Key })
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + "/producer", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProducerInvalidUpload()
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

        var request = _fixture
            .Build<UpdateProducerRequest>()
            .With(x => x.Uploads, new List<Guid>{ Guid.NewGuid() })
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + "/producer", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProducerNotFound()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        var upload = _fixture
            .Build<Upload>()
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateProducerRequest>()
            .With(x => x.Uploads, new List<Guid>{ upload.Key })
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + "/producer", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
