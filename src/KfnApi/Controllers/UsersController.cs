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
[Route("v1/users")]
public class UsersController : KfnControllerBase
{
    private readonly IUserService _userService;
    private readonly IWorkflowService _workflowService;
    private readonly ICloudStorageService _cloudService;

    public UsersController(IUserService userService, ICloudStorageService cloudService, IWorkflowService workflowService)
    {
        _userService = userService;
        _cloudService = cloudService;
        _workflowService = workflowService;
    }

    [HttpGet("profiles")]
    [RequirePermission(Permission.GetUserProfiles)]
    public async Task<IActionResult> GetProfilesAsync([FromQuery] GetAllProfilesRequest profilesRequest)
    {
        var request = new GetAllUsersRequest(profilesRequest);
        var paginated = await _userService.GetAllUsersAsync(request);
        var profiles = paginated.Select(user => user.ToProfileResponse(_cloudService)).ToList();
        return Ok(paginated.ToPaginatedResponse(profiles));
    }

    [HttpGet("profiles/{id:guid}")]
    [RequirePermission(Permission.GetUserProfileById)]
    public async Task<IActionResult> GetProfileAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id, activeOnly:true);

        return user is null
            ? NotFoundResponse()
            : Ok(user.ToProfileResponse(_cloudService));
    }

    [HttpGet]
    [RequirePermission(Permission.GetUsers)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetAllUsersRequest request)
    {
        var paginated = await _userService.GetAllUsersAsync(request);
        var users = paginated.Select(user => user.ToUserListResponse(_cloudService)).ToList();
        return Ok(paginated.ToPaginatedResponse(users));
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(Permission.GetUserById)]
    public async Task<IActionResult> GetUserAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        return user is null
            ? NotFoundResponse()
            : Ok(user.ToUserResponse(_cloudService));
    }

    [HttpPatch("{id:guid}")]
    [RequirePermission(Permission.UpdateUserState)]
    public async Task<IActionResult> UpdateUserStateAsync(Guid id, [FromBody] UpdateUserStateRequest request)
    {
        var result = await _workflowService.UpdateUserStateAsync(id, request);

        return result.IsSuccess()
            ? SuccessResponse(result.Value!.ToUserListResponse(_cloudService), result.HttpCode)
            : ErrorResponse(result.Error!);
    }
}
