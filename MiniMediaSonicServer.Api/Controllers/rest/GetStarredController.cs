using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetStarredController : SonicControllerBase
{
    private readonly ArtistService _artistService;
    private readonly AlbumService _albumService;
    private readonly TrackService _trackService;
    public GetStarredController(
        ArtistService artistService,
        AlbumService albumService,
        TrackService trackService)
    {
        _artistService = artistService;
        _albumService = albumService;
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        var artistsTask = _artistService.GetStarredArtistsAsync(User.UserId);
        var albumsTask = _albumService.GetStarredAlbumsAsync(User.UserId);
        var tracksTask = _trackService.GetStarredTracksAsync(User.UserId);
        await Task.WhenAll(artistsTask, albumsTask, tracksTask);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Starred = new StarredResponse
            {
                Artists = artistsTask.Result,
                Albums = albumsTask.Result,
                Tracks = tracksTask.Result
            }
        });
    }
}