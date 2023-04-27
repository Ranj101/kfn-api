using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Authorization.Policy;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/producers")]
public class ProducersController : KfnControllerBase
{
    private readonly IProducerService _service;
    private readonly IWorkflowService _workflowService;
    private readonly ICloudStorageService _cloudService;

    public ProducersController(IProducerService service, ICloudStorageService cloudService, IWorkflowService workflowService)
    {
        _service = service;
        _cloudService = cloudService;
        _workflowService = workflowService;
    }

    [HttpGet("pages")]
    [RequirePermission(Permission.GetProducerPages)]
    public async Task<IActionResult> GetPagesAsync([FromQuery] GetAllProducerPagesRequest pagesRequest)
    {
        var request = new GetAllProducersRequest(pagesRequest);
        var paginated = await _service.GetAllProducersAsync(request);
        var pages = paginated.Select(producer => producer.ToProducerPageListResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(pages));
    }

    [HttpGet("pages/{id:guid}")]
    [RequirePermission(Permission.GetProducerPageById)]
    public async Task<IActionResult> GetPageAsync(Guid id)
    {
        var producer = await _service.GetByIdAsync(id, activeOnly:true);

        return producer is null
            ? NotFoundResponse()
            : Ok(producer.ToProducerPageResponse(_cloudService));
    }

    [HttpGet]
    [RequirePermission(Permission.GetProducers)]
    public async Task<IActionResult> GetProducersAsync([FromQuery] GetAllProducersRequest request)
    {
        var paginated = await _service.GetAllProducersAsync(request);
        var pages = paginated.Select(producer => producer.ToProducerPageListResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(pages));
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(Permission.GetProducerById)]
    public async Task<IActionResult> GetProducerAsync(Guid id)
    {
        var producer = await _service.GetByIdAsync(id);

        return producer is null
            ? NotFoundResponse()
            : Ok(producer.ToProducerResponse(_cloudService));
    }

    [HttpPatch("{id:guid}")]
    [RequirePermission(Permission.UpdateProducerState)]
    public async Task<IActionResult> UpdateProducerStateAsync(Guid id, [FromBody] UpdateProducerStateRequest request)
    {
        var result = await _workflowService.UpdateProducerStateAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToProducerListResponse(), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    // TODO: Leave a review for a producer
}
