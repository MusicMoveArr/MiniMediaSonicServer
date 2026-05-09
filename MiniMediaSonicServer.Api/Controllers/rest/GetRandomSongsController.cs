using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetRandomSongsController : SonicControllerBase
{
    private readonly TrackService _trackService;
    private readonly MusicCacheService _musicCacheService;
    public GetRandomSongsController(TrackService trackService,
        MusicCacheService musicCacheService)
    {
        _trackService = trackService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetRandomSongsRequest request)
    {
        if (request.Size <= 0)
        {
            request.Size = 10;
        }
        if (request.Size > 500)
        {
            request.Size = 500;
        }

        var randomTracks = await _trackService.GetRandomTracksAsync(request.Size, User.UserId, request.Genre);
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in randomTracks)
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            RandomSongs = new RandomSongsResponse
            {
                Tracks = randomTracks
            }
        });
    }
}