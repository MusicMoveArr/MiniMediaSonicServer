
using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class CreateUserController : SonicControllerBase
{
    private readonly UserService _userService;
    public CreateUserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] CreateUserRequest request)
    {
        if (!GetUserModel().AdminRole)
        {
            return SubsonicResults.Fail(HttpContext, 0, "You are not authorized to create this user.");
        }
        bool success = await _userService.CreateUserAsync(request);
        if (!success)
        {
            return SubsonicResults.Fail(HttpContext, 0, "Another user by this username already exists.");
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
    
}