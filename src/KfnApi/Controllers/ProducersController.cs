using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
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
    private readonly ICloudStorageService _cloudService;

    public ProducersController(IProducerService service, ICloudStorageService cloudService)
    {
        _service = service;
        _cloudService = cloudService;
    }

    [HttpGet("pages")]
    public async Task<IActionResult> GetPagesAsync([FromQuery] GetAllProducerPagesRequest pagesRequest)
    {
        var request = new GetAllProducersRequest(pagesRequest);
        var paginated = await _service.GetAllProducersAsync(request);
        var pages = paginated.Select(producer => producer.ToProducerPageListResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(pages));
    }

    [HttpGet("pages/{id:guid}")]
    public async Task<IActionResult> GetProfileAsync(Guid id)
    {
        var producer = await _service.GetByIdAsync(id, activeOnly:true);

        return producer is null
            ? NotFoundResponse()
            : Ok(producer.ToProducerPageResponse(_cloudService));
    }

    [HttpGet]
    public async Task<IActionResult> GetProducersAsync([FromQuery] GetAllProducersRequest request)
    {
        var paginated = await _service.GetAllProducersAsync(request);
        var pages = paginated.Select(producer => producer.ToProducerPageListResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(pages));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProducerAsync(Guid id)
    {
        var producer = await _service.GetByIdAsync(id);

        return producer is null
            ? NotFoundResponse()
            : Ok(producer.ToProducerResponse(_cloudService));
    }

    // Create Producer
    // Update Producer
    // Deactivate Producer
    // Leave a review for a producer
}
