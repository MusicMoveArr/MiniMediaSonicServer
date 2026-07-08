using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetSonicSimilarTracksController : SonicControllerBase
{
    private readonly TrackService _trackService;
    private readonly MusicCacheService _musicCacheService;
    public GetSonicSimilarTracksController(TrackService trackService,
        MusicCacheService musicCacheService)
    {
        _trackService = trackService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSimilarSongsRequest request)
    {
        var tracks = await _trackService.GetSonicSimilarTracksAsync(request.Id, request.Count, User.UserId);
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in tracks)
            {
                track.Song.Path = _musicCacheService.GetExposedCachedPath(track.Song.Path, track.Song);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            SonicMatch = tracks
        });
    }
}