using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using IntegrationTests.Extensions;
using KfnApi;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.ControllerTests;

public class ProductsControllerTests : Base
{
    private const string BaseUrl = "v1/products";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public ProductsControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetProducts()
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

        var product = _fixture
            .Build<Product>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, ProductState.Available)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.Prices)
            .Create();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById()
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

        var product = _fixture
            .Build<Product>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, ProductState.Available)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.Prices)
            .Create();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{product.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct()
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
            .With(x => x.Key, Constants.DefaultProductPicture)
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<CreateProductRequest>()
            .Without(x => x.Picture)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PostAsJsonAsync(BaseUrl, request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateProduct()
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
            .With(x => x.Key, Constants.DefaultProductPicture)
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var product = _fixture
            .Build<Product>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, ProductState.Available)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.Prices)
            .Create();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<CreateProductRequest>()
            .Without(x => x.Picture)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + $"/{product.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduct()
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
            .With(x => x.Key, Constants.DefaultProductPicture)
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var product = _fixture
            .Build<Product>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, ProductState.Available)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.Prices)
            .Create();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();


        // ~~~ Act ~~~
        var httpResponse = await client.DeleteAsync(BaseUrl + $"/{product.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProductStateToUnavailable()
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
            .With(x => x.Key, Constants.DefaultProductPicture)
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var product = _fixture
            .Build<Product>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, ProductState.Available)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.Prices)
            .Create();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateProductStateRequest>()
            .With(x => x.Trigger, ExposedProductTrigger.MakeUnavailable)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{product.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProductStateToAvailable()
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
            .With(x => x.Key, Constants.DefaultProductPicture)
            .With(x => x.DateUploaded, DateTime.UtcNow)
            .Without(x => x.Users)
            .Without(x => x.Producers)
            .Without(x => x.Products)
            .Without(x => x.ApprovalForms)
            .Create();

        _dbContext.Uploads.Add(upload);
        await _dbContext.SaveChangesAsync();

        var product = _fixture
            .Build<Product>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, ProductState.Unavailable)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Without(x => x.UpdatedAt)
            .Without(x => x.Producer)
            .Without(x => x.Orders)
            .Without(x => x.Uploads)
            .Without(x => x.Prices)
            .Create();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateProductStateRequest>()
            .With(x => x.Trigger, ExposedProductTrigger.MakeAvailable)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{product.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
