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
[Route("v1/self")]
public class SelfController : KfnControllerBase
{
    private readonly ISelfService _selfService;
    private readonly IApprovalFormService _formService;
    private readonly IProducerService _producerService;
    private readonly ICloudStorageService _cloudService;

    public SelfController(ISelfService selfService, IApprovalFormService formService, IProducerService producerService, ICloudStorageService cloudService)
    {
        _selfService = selfService;
        _formService = formService;
        _cloudService = cloudService;
        _producerService = producerService;
    }

    [HttpGet]
    [RequirePermission(Permission.GetSelf)]
    public async Task<IActionResult> GetSelfAsync()
    {
        var user = await _selfService.GetSelfAsync();

        if (user is null)
            return NotFound();

        return Ok(user.ToSelfResponse(_cloudService));
    }

    [HttpPatch]
    [RequirePermission(Permission.UpdateSelf)]
    public async Task<IActionResult> UpdateSelfAsync([FromBody] UpdateSelfRequest request)
    {
        var result = await _selfService.UpdateSelfAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToSelfResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPut("form")]
    [RequirePermission(Permission.UpdateForm)]
    public async Task<IActionResult> UpdateFormAsync([FromBody] SubmitFormRequest request)
    {
        var result = await _formService.UpdateFormAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToFormResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }

    [HttpPut("producer")]
    [RequirePermission(Permission.UpdateProducer)]
    public async Task<IActionResult> UpdateProducerAsync([FromBody] UpdateProducerRequest request)
    {
        var result = await _producerService.UpdateProducerAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToProducerPageResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
