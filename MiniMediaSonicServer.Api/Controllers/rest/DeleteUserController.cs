using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class DeleteUserController : SonicControllerBase
{
    private readonly UserService _userService;
    public DeleteUserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] DeleteUserRequest request)
    {
        var currentUser = GetUserModel();
        if (currentUser.Username != request.Username && !currentUser.AdminRole)
        {
            return SubsonicResults.Fail(HttpContext, 0, "You are not authorized to update this user.");
        }
        await _userService.SetUserDeletedByUsernameAsync(currentUser.Username);

        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}