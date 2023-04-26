using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<PaginatedList<Order>> GetOrdersAsync(GetAllOrdersRequest request);
    Task<Result<Order>> SubmitOrderAsync(SubmitOrderRequest request);
    Task<Result<Order>> UpdateOrderAsync(Guid id, UpdateOrderRequest request);
}
