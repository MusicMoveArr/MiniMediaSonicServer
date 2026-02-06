using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetPlaylistsController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    public GetPlaylistsController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get()
    {
        var userModel = GetUserModel();
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Playlists = new Playlists
            {
                Playlist = (await _playlistService
                    .GetAllPlaylistsAsync(User.UserId))
                    .Select(playlist => new Playlist
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
                    }).ToList()
            }
        });
    }
}