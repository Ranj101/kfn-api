using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/self")]
public class SelfController : KfnControllerBase
{
    private readonly ISelfService _selfService;
    private readonly IApprovalFormService _formService;
    private readonly ICloudStorageService _cloudService;

    public SelfController(ISelfService selfService, IApprovalFormService formService, ICloudStorageService cloudService)
    {
        _selfService = selfService;
        _formService = formService;
        _cloudService = cloudService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSelfAsync()
    {
        var user = await _selfService.GetSelfAsync();

        if (user is null)
            return NotFound();

        return Ok(user.ToSelfResponse(_cloudService));
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateSelfAsync(UpdateSelfRequest request)
    {
        var result = await _selfService.UpdateSelfAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToSelfResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPut("forms/{id:guid}")]
    public async Task<IActionResult> UpdateFormAsync(Guid id, SubmitFormRequest request)
    {
        var result = await _formService.UpdateFormAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToFormResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
