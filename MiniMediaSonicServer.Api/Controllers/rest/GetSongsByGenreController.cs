using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetSongsByGenreController : SonicControllerBase
{
    private readonly TrackService _trackService;
    private readonly MusicCacheService _musicCacheService;
    public GetSongsByGenreController(TrackService trackService,
        MusicCacheService musicCacheService)
    {
        _trackService = trackService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSongsByGenreRequest request)
    {
        if (request.Count < 0)
        {
            request.Count = 10;
        }
        if (request.Count > 500)
        {
            request.Count = 500;
        }

        var tracks = await _trackService.GetTrackByGenreAsync(User.UserId, request.Genre, request.Count, request.Offset);
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in tracks)
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            SongsByGenre = new SongsByGenreResponse
            {
                Tracks = tracks
            }
        });
    }
}