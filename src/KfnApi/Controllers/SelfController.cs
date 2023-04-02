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
    private readonly ISelfService _service;
    private readonly ICloudStorageService _cloudService;

    public SelfController(ISelfService service, ICloudStorageService cloudService)
    {
        _service = service;
        _cloudService = cloudService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSelfAsync()
    {
        var user = await _service.GetSelfAsync();

        if (user is null)
            return NotFound();

        return Ok(user.ToSelfResponse(_cloudService));
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateSelfAsync(UpdateSelfRequest request)
    {
        var result = await _service.UpdateSelfAsync(request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToSelfResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
