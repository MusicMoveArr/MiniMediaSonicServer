using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class UpdateUserController : SonicControllerBase
{
    private readonly UserService _userRepository;
    public UpdateUserController(UserService userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] UpdateUserRequest request)
    {
        var currentUser = GetUserModel();
        if (currentUser.Username != request.Username && !currentUser.AdminRole)
        {
            return SubsonicResults.Fail(HttpContext, 0, "You are not authorized to update this user.");
        }
        
        await _userRepository.UpdateUserAsync(request);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}