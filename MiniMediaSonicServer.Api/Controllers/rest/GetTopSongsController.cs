using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetTopSongsController : SonicControllerBase
{
    private readonly TrackService _trackService;
    private readonly MusicCacheService _musicCacheService;
    public GetTopSongsController(TrackService trackService,
        MusicCacheService musicCacheService)
    {
        _trackService = trackService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetTopSongsRequest request)
    {
        if (request.Count <= 0)
        {
            request.Count = 20;
        }

        var tracks = await _trackService.GetTopTracksByArtistNameAsync(request.Artist, request.Count, User.UserId);
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in tracks)
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            TopSongs = new TopSongsResponse
            {
                Tracks = tracks
            }
        });
    }
}