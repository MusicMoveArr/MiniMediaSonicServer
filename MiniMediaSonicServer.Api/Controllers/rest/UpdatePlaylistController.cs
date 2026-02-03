using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class UpdatePlaylistController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    public UpdatePlaylistController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] UpdatePlaylistRequest request)
    {
        await _playlistService.UpdatePlaylistByIdAsync(request);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}