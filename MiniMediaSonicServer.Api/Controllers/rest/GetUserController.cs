using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetUserController : SonicControllerBase
{
    private readonly UserService _userService;
    public GetUserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetUserRequest request)
    {
        var user = GetUserModel();
        if (User.AdminRole && !string.Equals(request.Username, user.Username, StringComparison.OrdinalIgnoreCase))
        {
            var userdb = await _userService.GetUserByUsernameAsync(request.Username);

            if (userdb == null)
            {
                return SubsonicResults.Fail(HttpContext, 0, "User not found.");
            }
            
            user = GetUserModel(userdb);
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            User = user
        });
    }
}