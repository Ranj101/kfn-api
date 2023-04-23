using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/products")]
public class ProductsController : KfnControllerBase
{
    private readonly IProductService _service;
    private readonly IWorkflowService _workflowService;
    private readonly ICloudStorageService _cloudService;

    public ProductsController(IProductService service, ICloudStorageService cloudService, IWorkflowService workflowService)
    {
        _service = service;
        _cloudService = cloudService;
        _workflowService = workflowService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsAsync([FromQuery] GetProductsRequest request)
    {
        var paginated = await _service.GetProductsAsync(request);
        var pages = paginated.Select(product => product.ToProductListResponse(_cloudService)).ToList();
        return Ok(paginated.ToPaginatedResponse(pages));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductAsync(Guid id)
    {
        var product = await _service.GetByIdAsync(id);

        return product is null
            ? NotFoundResponse()
            : Ok(product.ToProductResponse(_cloudService));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequest request)
    {
        var result = await _service.CreateProductAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToProductResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProductAsync(Guid id, CreateProductRequest request)
    {
        var result = await _service.ResolveUpdateCriticality(id, request);

        if (!result.IsSuccess())
            return ErrorResponse(result.Error!);

        var (criticality, product) = result.Value!;

        if (criticality == ProductUpdate.NonCritical)
        {
            var updateResult = await _service.UpdateProductAsync(product, request);

            return updateResult.IsSuccess()
                ? SuccessResponse(updateResult.Value!.ToProductResponse(_cloudService), result.HttpCode)
                : ErrorResponse(updateResult.Error!);
        }

        var replicationResult = await _workflowService.SafeProductUpdateAsync(product, request);

        return replicationResult.IsSuccess()
            ? SuccessResponse(replicationResult.Value!.ToProductResponse(_cloudService), result.HttpCode)
            : ErrorResponse(replicationResult.Error!);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateProductStateAsync(Guid id, [FromBody] UpdateProductStateRequest request)
    {
        var result = await _workflowService.UpdateProductStateAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToProductResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductAsync(Guid id)
    {
        var result = await _service.DeleteProductAsync(id);

        if (!result.IsSuccess())
            return ErrorResponse(result.Error!);

        if (result.HttpCode != StatusCodes.Status102Processing)
            return SuccessResponse(result.Value!.ToProductResponse(_cloudService), result.HttpCode);

        var deleteResult = await _workflowService.SafeProductDeleteAsync(result.Value!);

        return deleteResult.IsSuccess()
            ? SuccessResponse(deleteResult.Value!.ToProductResponse(_cloudService), result.HttpCode)
            : ErrorResponse(deleteResult.Error!);
    }
}
