using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class AlbumService
{
    private readonly AlbumRepository _albumRepository;

    public AlbumService(AlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<List<AlbumID3>> GetAlbumList2ResponseAsync(GetAlbumList2Request request)
    {
        return await _albumRepository.GetAlbumId3Async(request);
    }

    public async Task<AlbumID3> GetAlbumByIdResponseAsync(Guid albumId)
    {
        return await _albumRepository.GetAlbumId3WithTracksAsync(albumId);
    }
}