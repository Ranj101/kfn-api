using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/forms")]
public class ApprovalFormsController : KfnControllerBase
{
    private readonly IApprovalFormService _service;
    private readonly IWorkflowService _workflowService;
    private readonly ICloudStorageService _cloudService;

    public ApprovalFormsController(IApprovalFormService service, ICloudStorageService cloudService, IWorkflowService workflowService)
    {
        _service = service;
        _cloudService = cloudService;
        _workflowService = workflowService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFormsAsync([FromQuery] GetAllFormsRequest request)
    {
        var paginated = await _service.GetAllFormsAsync(request);
        var forms = paginated.Select(form => form.ToFormListResponse()).ToList();

        return Ok(paginated.ToPaginatedResponse(forms));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFormAsync(Guid id)
    {
        var form = await _service.GetByIdAsync(id);

        return form is null
            ? NotFoundResponse()
            : Ok(form.ToFormResponse(_cloudService));
    }

    [HttpPost]
    public async Task<IActionResult> SubmitFormAsync([FromBody] SubmitFormRequest request)
    {
        var result = await _service.CreateFormAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToFormResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateFormStateAsync(Guid id, [FromBody] UpdateFormStateRequest request)
    {
        var result = await _workflowService.UpdateFormStateAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToFormResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
