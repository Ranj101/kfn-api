using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/orders")]
public class OrdersController : KfnControllerBase
{
    private readonly IOrderService _service;
    private readonly IWorkflowService _workflowService;
    private readonly ICloudStorageService _cloudService;

    public OrdersController(IOrderService service, ICloudStorageService cloudService, IWorkflowService workflowService)
    {
        _service = service;
        _cloudService = cloudService;
        _workflowService = workflowService;
    }

    [HttpGet("basic")]
    public async Task<IActionResult> GetBasicOrdersAsync([FromQuery] GetAllBasicOrdersRequest basicRequest)
    {
        var request = new GetAllOrdersRequest(basicRequest);
        var paginated = await _service.GetOrdersAsync(request);
        var basicOrders = paginated.Select(order => order.ToOrderBasicListResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(basicOrders));
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] GetAllOrdersRequest request)
    {
        var paginated = await _service.GetOrdersAsync(request);
        var orders = paginated.Select(order => order.ToOrderListResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(orders));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderAsync(Guid id)
    {
        var order = await _service.GetByIdAsync(id);

        return order is null
            ? NotFoundResponse()
            : Ok(order.ToOrderResponse(_cloudService));
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrderAsync(SubmitOrderRequest request)
    {
        var result = await _service.SubmitOrderAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToOrderResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrderAsync(Guid id, UpdateOrderRequest request)
    {
        var result = await _service.UpdateOrderAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToOrderResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateOrderStateAsync(Guid id, [FromBody] UpdateOrderStateRequest request)
    {
        var result = await _workflowService.UpdateOrderStateAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToOrderResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
