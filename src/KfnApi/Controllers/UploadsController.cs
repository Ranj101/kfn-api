using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Authorization.Policy;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/uploads")]
public class UploadsController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadsController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost]
    [RequirePermission(Permission.UploadFile)]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, CancellationToken token)
    {
        var upload = await _uploadService.UploadAsync(file, token);
        return Ok(upload.ToUploadResponse());
    }
}
