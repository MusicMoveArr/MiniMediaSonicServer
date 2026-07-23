using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetAlbumController : SonicControllerBase
{
    private readonly AlbumService _albumService;
    private readonly MusicCacheService _musicCacheService;
    public GetAlbumController(AlbumService albumService,
        MusicCacheService musicCacheService)
    {
        _albumService = albumService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetAlbumRequest request)
    {
        var album = await _albumService.GetAlbumByIdResponseAsync(request.Id, User.UserId);

        if (album == null)
        {
            return SubsonicResults.Fail(HttpContext, SubsonicErrorCode.DataNotFound, "Album not found");
        }
        
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in album.Song)
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            Album = album
        });
    }
}