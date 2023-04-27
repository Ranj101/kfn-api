using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;

namespace KfnApi.Abstractions;

public interface IProductService
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<PaginatedList<Product>> GetProductsAsync(GetProductsRequest request);
    Task<Result<Product>> CreateProductAsync(CreateProductRequest request);
    Task<Result<Product>> UpdateProductAsync(Product product, CreateProductRequest request);
    Task<Result<Tuple<ProductUpdate, Product>>> ResolveUpdateCriticality(Guid id, CreateProductRequest request);
    Task<Result<Product>> DeleteProductAsync(Guid id);
}
