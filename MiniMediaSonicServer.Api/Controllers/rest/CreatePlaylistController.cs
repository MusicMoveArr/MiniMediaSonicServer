using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class CreatePlaylistController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    public CreatePlaylistController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] CreatePlaylistRequest request)
    {
        var userModel = GetUserModel();
        var playlist = await _playlistService.CreatePlaylistAsync(User.UserId, request.Name);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            Playlist = new Playlist
            {
                Name = playlist.Name,
                SongCount =  playlist.SongCount,
                Changed = playlist.UpdatedAt,
                Created = playlist.CreatedAt,
                Duration = playlist.TotalDuration,
                Id = playlist.PlaylistId,
                Owner = userModel.Username,
                Public = playlist.Public,
                Entry = playlist.Tracks
            }
        });
    }
}