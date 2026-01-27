using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetPlaylistController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    public GetPlaylistController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetPlaylistRequest request)
    {
        var userModel = GetUserModel();
        var playlist = await _playlistService.GetPlaylistByIdAsync(request.Id);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
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