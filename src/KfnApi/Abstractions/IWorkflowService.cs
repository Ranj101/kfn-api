using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IWorkflowService
{
    Task<Result<User>> UpdateUserStateAsync(Guid id, UpdateUserStateRequest request);
    Task<Result<Producer>> UpdateProducerStateAsync(Guid id, UpdateProducerStateRequest request);
    Task<Result<ApprovalForm>> UpdateFormStateAsync(Guid id, UpdateFormStateRequest request);
    Task<Result<Product>> UpdateProductStateAsync(Guid id, UpdateProductStateRequest request);
    Task<Result<Product>> SafeProductUpdateAsync(Product product, CreateProductRequest request);
    Task<Result<Product>> SafeProductDeleteAsync(Product product);
    Task ExpireOrdersAsync(List<Order> orders);
    Task<Result<Order>> UpdateOrderStateAsync(Guid id, UpdateOrderStateRequest request);
}
