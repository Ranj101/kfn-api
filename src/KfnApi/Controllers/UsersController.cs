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

        return user is null
            ? NotFoundResponse()
            : Ok(user.ToProfileResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetAllUsersRequest request)
    {
        var paginated = await _service.GetAllUsersAsync(request);
        var users = paginated.Select(user => user.ToUserResponse()).ToList();
        return Ok(paginated.ToPaginatedResponse(users));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserAsync(Guid id)
    {
        var user = await _service.GetByIdAsync(id);

        return user is null
            ? NotFoundResponse()
            : Ok(user.ToUserResponse());
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUserStateAsync(Guid id, UpdateUserStateRequest request)
    {
        var result = await _service.UpdateUserState(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value, result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
