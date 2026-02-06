using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetUsersController : SonicControllerBase
{
    private readonly UserService _userService;
    public GetUsersController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get()
    {
        if (!User.AdminRole)
        {
            return SubsonicResults.Fail(HttpContext, 0, "No permissions.");
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Users = new UserListResponse
            {
                User = (await _userService.GetAllUsersAsync())
                    .Select(user => GetUserModel(user))
                    .ToList()
            }
        });
    }
}