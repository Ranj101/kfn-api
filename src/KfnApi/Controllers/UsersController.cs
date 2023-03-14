using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;


[Authorize]
[ApiController]
[Route("v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("self")]
    public async Task<IActionResult> GetSelfAsync()
    {
        throw new NotImplementedException();
    }

    [HttpPatch("self")]
    public async Task<ActionResult> UpdateSelfAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("profiles")]
    public async Task<IActionResult> GetProfilesAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("profiles/{id}")]
    public async Task<IActionResult> GetProfileAsync(string id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetAllUsersRequest request)
    {
        var paginated = await _service.GetAllUsersAsync(request);
        return Ok(paginated.ToPaginatedResponse(paginated.ToList()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserAsync(string id)
    {
        throw new NotImplementedException();
    }
}
