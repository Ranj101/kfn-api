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

public class ProductService : IProductService
{
    private static readonly Dictionary<SortProductBy, ISortBy> SortFunctions = new ()
    {
        { SortProductBy.DateCreated, new SortBy<Product, DateTime>(x => x.CreatedAt) }
    };

    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public ProductService(DatabaseContext databaseContext, IAuthContext authContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _databaseContext.Products
            .Include(p => p.Prices)
            .Include(p => p.Producer)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PaginatedList<Product>> GetProductsAsync(GetProductsRequest request)
    {
        var nameFilter = request.FilterByName is null;
        var producerFilter = request.FilterByProducer is null;

        var products = _databaseContext.Products
            .Include(p => p.Prices)
            .Include(p => p.Producer)
            .Where(product => (product.State != ProductState.Modified) &&
                              (producerFilter || product.ProducerId == request.FilterByProducer) &&
                              (nameFilter || product.Name.ToLower().Contains(request.FilterByName!.Trim().ToLower())))
            .AsQueryable();

        products = request.SortDirection == SortDirection.Descending
            ? products.SortByDescending(SortFunctions[request.SortBy])
            : products.SortBy(SortFunctions[request.SortBy]);

        var paginated = await PaginatedList<Product>.CreateAsync(products, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<Result<Product>> CreateProductAsync(CreateProductRequest request)
    {
        var producer = await _databaseContext.Producers.FirstOrDefaultAsync(p =>
            p.UserId == _authContext.GetUserId() && p.State == ProducerState.Active);

        if (producer is null)
            return Result<Product>.NotFoundResult();

        var uploads = new List<Upload>();

        var hasPicture = request.Picture.HasValue;

        if (hasPicture)
        {
            uploads = await TryAddUploadAsync(uploads, request.Picture!.Value);
        }
        else
        {
            uploads = await TryAddUploadAsync(uploads, Constants.DefaultProductPicture);
        }

        if (!uploads.Any())
            return Result<Product>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Creation Failed",
                Detail = "Request contains invalid upload."
            });

        var productId = Guid.NewGuid();
        var prices = request.Prices.Select(price => new PriceByWeight
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Value = price.Value,
            Weight = price.Weight
        }).ToList();

        var product = new Product
        {
            Id = productId,
            ProducerId = producer.Id,
            Name = request.Name,
            Picture = request.Picture ?? Constants.DefaultProductPicture,
            State = ProductState.Available,
            CreatedBy = _authContext.GetUserId(),
            Prices = prices,
            Uploads = uploads
        };

        await _databaseContext.Products.AddAsync(product);
        await _databaseContext.SaveChangesAsync();

        return Result<Product>.SuccessResult(product, StatusCodes.Status201Created);
    }

    // handles non-critical updates
    public async Task<Result<Product>> UpdateProductAsync(Product product, CreateProductRequest request)
    {
        var uploads = new List<Upload>();

        var hasPicture = request.Picture.HasValue;

        if (hasPicture)
        {
            uploads = await TryAddUploadAsync(uploads, request.Picture!.Value);
        }
        else
        {
            uploads = await TryAddUploadAsync(uploads, Constants.DefaultProductPicture);
        }

        if (!uploads.Any())
            return Result<Product>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Update Failed",
                Detail = "Request contains invalid upload."
            });

        product.Picture = request.Picture ?? Constants.DefaultProductPicture;
        product.Uploads = uploads;

        await _databaseContext.SaveChangesAsync();
        return Result<Product>.SuccessResult(product, StatusCodes.Status200OK);
    }

    public async Task<Result<Tuple<ProductUpdate, Product>>> ResolveUpdateCriticality(Guid id, CreateProductRequest request)
    {
        var producer = await _databaseContext.Producers.FirstOrDefaultAsync(p =>
            p.UserId == _authContext.GetUserId() && p.State == ProducerState.Active);

        if (producer is null)
            return Result<Tuple<ProductUpdate, Product>>.NotFoundResult();

        var product = await _databaseContext.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == id && p.ProducerId == producer.Id);

        if(product is null)
            return Result<Tuple<ProductUpdate, Product>>.NotFoundResult();

        if (product.Name != request.Name || product.Prices!.Count != request.Prices.Count)
            return Result<Tuple<ProductUpdate, Product>>.SuccessResult(new(ProductUpdate.Critical, product), StatusCodes.Status200OK);

        return request.Prices.Select(price => product.Prices
            .Any(p => Math.Abs(p.Value - price.Value) < 0.0000001 && Math.Abs(p.Weight - price.Weight) < 0.0000001)).Any(check => check == false)
            ? Result<Tuple<ProductUpdate, Product>>.SuccessResult(new(ProductUpdate.Critical, product), StatusCodes.Status200OK)
            : Result<Tuple<ProductUpdate, Product>>.SuccessResult(new(ProductUpdate.NonCritical, product), StatusCodes.Status200OK);
    }

    public async Task<Result<Product>> DeleteProductAsync(Guid id)
    {
        var producer = await _databaseContext.Producers.FirstOrDefaultAsync(p =>
            p.UserId == _authContext.GetUserId() && p.State == ProducerState.Active);

        if (producer is null)
            return Result<Product>.NotFoundResult();

        var product = await _databaseContext.Products
            .Include(p => p.Prices)
            .Include(p => p.Producer)
            .Include(p => p.Orders)
            .FirstOrDefaultAsync(p => p.Id == id && p.ProducerId == producer.Id);

        if(product is null)
            return Result<Product>.NotFoundResult();

        if (product.Orders!.Any())
            return Result<Product>.SuccessResult(product, StatusCodes.Status102Processing);

        _databaseContext.Products.Remove(product);

        await _databaseContext.SaveChangesAsync();
        return Result<Product>.SuccessResult(product, StatusCodes.Status200OK);
    }

    private async Task<List<Upload>> TryAddUploadAsync(List<Upload> uploads, Guid key)
    {
        var upload = await _databaseContext.Uploads.FirstOrDefaultAsync(u => u.Key == key);

        if(upload is not null)
            uploads.Add(upload);

        return uploads;
    }
}
