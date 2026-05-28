using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetPlayQueueController : SonicControllerBase
{
    private readonly UserPlayQueueService _userPlayQueueService;
    public GetPlayQueueController(UserPlayQueueService userPlayQueueService)
    {
        _userPlayQueueService = userPlayQueueService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        var queue = await _userPlayQueueService.GetUserPlayQueueAsync(User.UserId);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            PlayQueue = new PlayQueue
            {
                Username = queue?.Username,
                ChangedBy = queue?.UpdatedByAppName,
                CurrentTrackId = queue?.CurrentTrackId,
                Changed = queue?.UpdatedAt,
                TrackPosition = queue?.TrackPosition ?? 0,
                Tracks = queue?.Tracks?.Select(track => track.Track).ToList() ?? []
            }
        });
    }
}