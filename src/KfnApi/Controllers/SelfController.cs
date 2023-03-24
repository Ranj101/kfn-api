using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/self")]
public class SelfController : ControllerBase
{
    private readonly ISelfService _service;

    public SelfController(ISelfService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetSelfAsync()
    {
        var user = await _service.GetSelfAsync();

        if (user is null)
            return NotFound();

        return Ok(user.ToUserResponse());
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateSelfAsync()
    {
        throw new NotImplementedException();
    }
}
