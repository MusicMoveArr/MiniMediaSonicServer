using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetSimilarSongs2Controller : SonicControllerBase
{
    private readonly TrackService _trackService;
    private readonly MusicCacheService _musicCacheService;
    public GetSimilarSongs2Controller(TrackService trackService,
        MusicCacheService musicCacheService)
    {
        _trackService = trackService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSimilarSongs2Request request)
    {
        var tracks = await _trackService.GetAlbumList2ResponseAsync(request.Id, request.Count, User.UserId);
        
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in tracks)
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            SimilarSongsList2 = new SimilarSongsList2Response
            {
                Tracks = tracks
            }
        });
    }
}