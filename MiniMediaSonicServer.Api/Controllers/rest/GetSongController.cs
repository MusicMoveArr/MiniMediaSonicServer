using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetSongController : SonicControllerBase
{
    private readonly TrackService _trackService;
    private readonly MusicCacheService _musicCacheService;
    public GetSongController(TrackService trackService,
        MusicCacheService musicCacheService)
    {
        _trackService = trackService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSongRequest request)
    {
        var track = await _trackService.GetTrackByIdAsync(request.Id, User.UserId);
        if (track != null)
        {
            track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Song = track
        });
    }
}