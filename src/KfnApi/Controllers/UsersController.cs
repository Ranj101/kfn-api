using KfnApi.Abstractions;
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

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        return Ok(await _service.GetAllUsersAsync());
    }
}
