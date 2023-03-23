using KfnApi.Abstractions;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/users")]
public class UsersController : KfnControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("profiles")]
    public async Task<IActionResult> GetProfilesAsync([FromQuery] GetAllUsersRequest request)
    {
        var paginated = await _service.GetAllUsersAsync(request);
        var profiles = paginated.Select(user => user.ToProfileResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(profiles));
    }

    [HttpGet("profiles/{id:guid}")]
    public async Task<IActionResult> GetProfileAsync(Guid id)
    {
        var user = await _service.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user.ToProfileResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetAllUsersRequest request)
    {
        var paginated = await _service.GetAllUsersAsync(request);
        return Ok(paginated.ToPaginatedResponse(paginated.ToList()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserAsync(Guid id)
    {
        var user = await _service.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUserStateAsync(Guid id, UpdateUserStateRequest request)
    {
        var result = await _service.UpdateUserState(id, request);

        return result.IsSuccess()
            ? Success(result.Value, result.HttpCode)
            : Failure(result.Error!);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> SubmitAbuseReportAsync(Guid id, string abuseReport)
    {
        throw new NotImplementedException();
    }

    // Block/Unblock user account
}
