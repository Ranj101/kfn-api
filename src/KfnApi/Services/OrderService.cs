using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Services;

public class OrderService : IOrderService
{
    private static readonly Dictionary<SortOrderBy, ISortBy> SortFunctions = new ()
    {
        { SortOrderBy.DateCreated, new SortBy<Order, DateTime>(x => x.CreatedAt) },
        { SortOrderBy.TotalPrice, new SortBy<Order, double>(x => x.TotalPrice) }
    };

    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public OrderService(DatabaseContext databaseContext, IAuthContext authContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        if (_authContext.IsAdmin())
            return await _databaseContext.Orders
                .Include(o => o.Products)!.ThenInclude(p => p.Prices)
                .Include(o => o.Products)!.ThenInclude(p => p.Producer)
                .FirstOrDefaultAsync(o => o.Id == id);

        if (_authContext.IsProducer())
        {
            var producer = await _databaseContext.Producers
                .FirstOrDefaultAsync(p =>
                p.UserId == _authContext.GetUserId() && p.State == ProducerState.Active);

            if (producer is null)
                return null;

            var order = await _databaseContext.Orders
                .Include(o => o.Products)!.ThenInclude(p => p.Prices)
                .FirstOrDefaultAsync(o => o.Id == id && o.ProducerId == producer.Id);

            return order;
        }

        return await _databaseContext.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == _authContext.GetUserId());
    }

    // TODO: Fix permission issues
    public async Task<PaginatedList<Order>> GetOrdersAsync(GetAllOrdersRequest request)
    {
        var stateFilter = request.FilterByState is null;
        var emailFilter = request.FilterByUserEmail is null;
        var userFilter = request.FilterByUserId is null;
        var producerFilter = request.FilterByProducerId is null;

        var orders = _databaseContext.Orders
            .Where(order => (emailFilter || order.User!.Email.ToLower().Contains(request.FilterByUserEmail!.Trim().ToLower())) &&
                            (stateFilter || order.State == request.FilterByState) &&
                            (userFilter || order.UserId == request.FilterByUserId) &&
                            (producerFilter || order.ProducerId == request.FilterByProducerId))
            .AsQueryable();

        orders = request.SortDirection == SortDirection.Descending
            ? orders.SortByDescending(SortFunctions[request.SortBy])
            : orders.SortBy(SortFunctions[request.SortBy]);

        var paginated = await PaginatedList<Order>.CreateAsync(orders, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<Result<Order>> SubmitOrderAsync(SubmitOrderRequest request)
    {
        var producer = await _databaseContext.Producers.FirstOrDefaultAsync(p =>
            p.Id == request.ProducerId!.Value && p.State == ProducerState.Active);

        if (producer is null)
            return Result<Order>.NotFoundResult();

        var products = new List<Product>();
        var prices = new List<PriceByWeight>();

        foreach (var orderedProduct in request.Products)
        {
            var product = await _databaseContext.Products
                .FirstOrDefaultAsync(p => p.Id == orderedProduct.ProductId && p.ProducerId == producer.Id);

            var price = await _databaseContext.Prices
                .FirstOrDefaultAsync(p => p.Id == orderedProduct.PriceByWeightId && p.ProductId == orderedProduct.ProductId);

            if(product is null || price is null)
                continue;

            prices.Add(price);
            products.Add(product);
        }

        if(products.Count != request.Products.Count)
            return Result<Order>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Submission Failed",
                Detail = "List contains invalid product or price."
            });

        var totalPrice = prices.Select(p => p.Value).Aggregate((total, next) => total + next);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _authContext.GetUserId(),
            ProducerId = producer.Id,
            TotalPrice = totalPrice,
            Location = request.Location,
            PickupTime = request.PickupTime!.Value,
            State = OrderState.Pending,
            CreatedBy = _authContext.GetUserId(),
            Products = products
        };

        await _databaseContext.Orders.AddAsync(order);
        await _databaseContext.SaveChangesAsync();

        return Result<Order>.SuccessResult(order, StatusCodes.Status201Created);
    }

    public async Task<Result<Order>> UpdateOrderAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _databaseContext.Orders
            .Include(o => o.Products)!.ThenInclude(p => p.Producer)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == _authContext.GetUserId() && o.State == OrderState.Pending);

        if (order is null)
            return Result<Order>.NotFoundResult();

        var products = new List<Product>();
        var prices = new List<PriceByWeight>();

        foreach (var orderedProduct in request.Products)
        {
            var product = await _databaseContext.Products
                .FirstOrDefaultAsync(p => p.Id == orderedProduct.ProductId && p.ProducerId == order.ProducerId);

            var price = await _databaseContext.Prices
                .FirstOrDefaultAsync(p => p.Id == orderedProduct.PriceByWeightId && p.ProductId == orderedProduct.ProductId);

            if(product is null || price is null)
                continue;

            prices.Add(price);
            products.Add(product);
        }

        if(products.Count != request.Products.Count)
            return Result<Order>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Submission Failed",
                Detail = "List contains invalid product or price."
            });

        var totalPrice = prices.Select(p => p.Value).Aggregate((total, next) => total + next);

        order.Location = request.Location;
        order.PickupTime = request.PickupTime;
        order.TotalPrice = totalPrice;
        order.Products = products;
        order.UpdatedBy = _authContext.GetUserId();

        await _databaseContext.SaveChangesAsync();
        return Result<Order>.SuccessResult(order, StatusCodes.Status200OK);
    }
}
