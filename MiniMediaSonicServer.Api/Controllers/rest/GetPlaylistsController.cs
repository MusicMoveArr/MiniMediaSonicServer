using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetPlaylistsController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    private readonly MusicCacheService _musicCacheService;
    public GetPlaylistsController(PlaylistService playlistService,
        MusicCacheService musicCacheService)
    {
        _playlistService = playlistService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        var userModel = GetUserModel();

        var playlists = (await _playlistService
                .GetAllPlaylistsAsync(User.UserId))
                .Select(playlist => new Playlist
                {
                    Name = playlist.Name,
                    SongCount = playlist.SongCount,
                    Changed = playlist.UpdatedAt,
                    Created = playlist.CreatedAt,
                    Duration = playlist.TotalDuration,
                    Id = playlist.PlaylistId,
                    Owner = userModel.Username,
                    Public = playlist.Public,
                    Entry = playlist.Tracks,
                    CoverArt = playlist.CoverArt
                })
                .ToList();
        
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in playlists.SelectMany(p => p.Entry))
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Playlists = new Playlists
            {
                Playlist = playlists
            }
        });
    }
}