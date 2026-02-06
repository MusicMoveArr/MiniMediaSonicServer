using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class DeletePlaylistController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    public DeletePlaylistController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] DeletePlaylistRequest request)
    {
        await _playlistService.SetPlaylistDeletedAsync(request.Id);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}