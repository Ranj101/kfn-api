using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/users")]
public class UsersController : KfnControllerBase
{
    private readonly IUserService _userService;
    private readonly ICloudStorageService _cloudService;

    public UsersController(IUserService userService, ICloudStorageService cloudService)
    {
        _userService = userService;
        _cloudService = cloudService;
    }

    [HttpGet("profiles")]
    public async Task<IActionResult> GetProfilesAsync([FromQuery] GetAllProfilesRequest profilesRequest)
    {
        var request = new GetAllUsersRequest(profilesRequest);
        var paginated = await _userService.GetAllUsersAsync(request);
        var profiles = paginated.Select(user => user.ToProfileResponse(_cloudService)).ToList();
        return Ok(paginated.ToPaginatedResponse(profiles));
    }

    [HttpGet("profiles/{id:guid}")]
    public async Task<IActionResult> GetProfileAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id, activeOnly:true);

        return user is null
            ? NotFoundResponse()
            : Ok(user.ToProfileResponse(_cloudService));
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetAllUsersRequest request)
    {
        var paginated = await _userService.GetAllUsersAsync(request);
        var users = paginated.Select(user => user.ToUserListResponse(_cloudService)).ToList();
        return Ok(paginated.ToPaginatedResponse(users));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        return user is null
            ? NotFoundResponse()
            : Ok(user.ToUserResponse(_cloudService));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUserStateAsync(Guid id, [FromBody] UpdateUserStateRequest request)
    {
        var result = await _userService.UpdateUserStateAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToUserListResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
