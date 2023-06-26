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

public class ApprovalFormsControllerTests : Base
{
    private const string BaseUrl = "v1/forms";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public ApprovalFormsControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetForms()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

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

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFormById()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

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

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{form.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SubmitForm()
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
        var httpResponse = await client.PostAsJsonAsync(BaseUrl, request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateFormStateToApproved()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

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

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateFormStateRequest>()
            .With(x => x.Trigger, ApprovalFormTrigger.Approve)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{form.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateFormStateToDeclined()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

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

        var client = GetClient(admin.IdentityId);

        var request = _fixture
            .Build<UpdateFormStateRequest>()
            .With(x => x.Trigger, ApprovalFormTrigger.Decline)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{form.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
