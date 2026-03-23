using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class SavePlayQueueController : SonicControllerBase
{
    private readonly UserPlayQueueService _userPlayQueueService;
    public SavePlayQueueController(UserPlayQueueService userPlayQueueService)
    {
        _userPlayQueueService = userPlayQueueService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery][FromBody] SavePlayQueueRequest request)
    {
        await _userPlayQueueService.SaveUserPlayQueueAsync(request, User.UserId, User.ClientName);
        await _userPlayQueueService.SaveUserPlayQueueTracksAsync(request, User.UserId);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}