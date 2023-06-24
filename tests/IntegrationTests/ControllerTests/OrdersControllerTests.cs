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

public class OrdersControllerTests : Base
{
    private const string BaseUrl = "v1/orders";

    private readonly Fixture _fixture = new();
    private readonly DatabaseContext _dbContext;

    public OrdersControllerTests(TestApplicationFactory factory) : base(factory)
    {
        _dbContext = factory.Services.GetRequiredService<DatabaseContext>();
    }

    [Fact]
    public async Task GetBasicOrders()
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

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Pending)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + "/basic");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrders()
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

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Pending)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrderById()
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

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Pending)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(admin.IdentityId);

        // ~~~ Act ~~~
        var httpResponse = await client.GetAsync(BaseUrl + $"/{order.Id}");

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SubmitOrder()
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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var productRequest = new OrderedProductRequest
        {
            ProductId = product.Id,
            PriceByWeightId = priceByWeight.Id
        };

        var request = _fixture
            .Build<SubmitOrderRequest>()
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.Products, new List<OrderedProductRequest> { productRequest })
            .With(x => x.PickupTime, DateTime.UtcNow)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PostAsJsonAsync(BaseUrl, request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

     [Fact]
    public async Task UpdateOrder()
    {
        // ~~~ Arrange ~~~
        _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(customer.IdentityId);

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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Pending)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var productRequest = new OrderedProductRequest
        {
            ProductId = product.Id,
            PriceByWeightId = priceByWeight.Id
        };

        var request = _fixture
            .Build<UpdateOrderRequest>()
            .With(x => x.Products, new List<OrderedProductRequest> { productRequest })
            .With(x => x.PickupTime, DateTime.UtcNow)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PutAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStateToApproved()
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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Pending)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateOrderStateRequest>()
            .With(x => x.Trigger, ExposedOrderTrigger.Approve)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStateToDeclined()
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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Pending)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateOrderStateRequest>()
            .With(x => x.Trigger, ExposedOrderTrigger.Decline)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStateToTerminated()
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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Approved)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateOrderStateRequest>()
            .With(x => x.Trigger, ExposedOrderTrigger.Terminate)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStateToFailed()
    {
       // ~~~ Arrange ~~~
       _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(customer.IdentityId);

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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Approved)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateOrderStateRequest>()
            .With(x => x.Trigger, ExposedOrderTrigger.Fail)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStateToCancelled()
    {
       // ~~~ Arrange ~~~
       _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(customer.IdentityId);

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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Approved)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateOrderStateRequest>()
            .With(x => x.Trigger, ExposedOrderTrigger.Cancel)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStateToConcluded()
    {
       // ~~~ Arrange ~~~
       _dbContext.ClearData();
        var admin = CreateAdmin();
        var customer = CreateCustomer();

        _dbContext.Users.Add(admin);
        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var client = GetClient(customer.IdentityId);

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

        var priceByWeight = _fixture
            .Build<PriceByWeight>()
            .With(x => x.ProductId, product.Id)
            .Without(x => x.Product)
            .Create();

        _dbContext.Prices.Add(priceByWeight);
        await _dbContext.SaveChangesAsync();

        var order = _fixture
            .Build<Order>()
            .With(x => x.UserId, customer.Id)
            .With(x => x.ProducerId, producer.Id)
            .With(x => x.State, OrderState.Approved)
            .With(x => x.PickupTime, DateTime.UtcNow)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.Products, new List<Product> { product })
            .Without(x => x.UpdatedAt)
            .Without(x => x.User)
            .Without(x => x.Producer)
            .Create();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = _fixture
            .Build<UpdateOrderStateRequest>()
            .With(x => x.Trigger, ExposedOrderTrigger.Conclude)
            .Create();

        // ~~~ Act ~~~
        var httpResponse = await client.PatchAsJsonAsync(BaseUrl + $"/{order.Id}", request);

        // ~~~ Assert ~~~
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
