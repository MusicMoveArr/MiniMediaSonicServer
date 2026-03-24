using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.Application.Utils;

namespace MiniMediaSonicServer.Application.Services;

public class AlbumService
{
    private readonly AlbumRepository _albumRepository;

    public AlbumService(AlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<List<AlbumID3>> GetAlbumList2ResponseAsync(GetAlbumList2Request request, Guid userId)
    {
        var albums = await _albumRepository.GetAlbumId3Async(request, userId);
        AlbumReleaseTypeUtil.SetAlbumReleaseTypes(albums);
        return albums;
    }

    public async Task<AlbumID3> GetAlbumByIdResponseAsync(Guid albumId, Guid userId)
    {
        var albums = await _albumRepository.GetAlbumId3WithTracksAsync(albumId, userId);
        AlbumReleaseTypeUtil.SetAlbumReleaseTypes(albums);
        return albums;
    }

    public async Task<List<AlbumID3>> GetStarredAlbumsAsync(Guid userId)
    {
        var albums = await _albumRepository.GetStarredAlbumsAsync(userId);
        AlbumReleaseTypeUtil.SetAlbumReleaseTypes(albums);
        return albums;
    }
}