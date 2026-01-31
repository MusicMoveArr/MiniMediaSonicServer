using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
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
    public async Task<IResult> Get()
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            starred = new StarredResponse
            {
                Artists = await _artistService.GetStarredArtistsAsync(User.UserId),
                Albums = await _albumService.GetStarredAlbumsAsync(User.UserId),
                Tracks = await _trackService.GetStarredTracksAsync(User.UserId)
            }
        });
    }
}