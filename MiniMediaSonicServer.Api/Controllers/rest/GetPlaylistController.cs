using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetPlaylistController : SonicControllerBase
{
    private readonly PlaylistService _playlistService;
    private readonly MusicCacheService _musicCacheService;
    public GetPlaylistController(PlaylistService playlistService,
        MusicCacheService musicCacheService)
    {
        _playlistService = playlistService;
        _musicCacheService = musicCacheService;
    }

    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetPlaylistRequest request)
    {
        var userModel = GetUserModel();
        var playlistRecord = await _playlistService.GetPlaylistByIdAsync(request.Id, User.UserId);

        if (playlistRecord == null)
        {
            return SubsonicResults.Fail(HttpContext, SubsonicErrorCode.DataNotFound, "Playlist not found");
        }

        var playlist = new Playlist
        {
            Name = playlistRecord.Name,
            SongCount = playlistRecord.SongCount,
            Changed = playlistRecord.UpdatedAt,
            Created = playlistRecord.CreatedAt,
            Duration = playlistRecord.TotalDuration,
            Id = playlistRecord.PlaylistId,
            Owner = userModel.Username,
            Public = playlistRecord.Public,
            Entry = playlistRecord.Tracks,
            ReadOnly = playlistRecord.ReadOnly,
            CoverArt = playlistRecord.CoverArt
        };
        
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var track in playlist.Entry)
            {
                track.Path = _musicCacheService.GetExposedCachedPath(track.Path, track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            Playlist = playlist
        });
    }
}