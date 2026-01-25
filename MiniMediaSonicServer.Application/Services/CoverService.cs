using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class CoverService
{
    private readonly TrackCoverRepository _trackCoverRepository;

    public CoverService(TrackCoverRepository trackCoverRepository)
    {
        _trackCoverRepository = trackCoverRepository;
    }

    public async Task<byte[]> GetAlbumCoverByAlbumIdAsync(Guid albumId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByAlbumIdAsync(albumId);

        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory)
            .DistinctBy(dir => dir.Name)
            .SelectMany(dir => dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            .Select(dir => dir)
            .FirstOrDefault();

        if (coverFileInfo != null)
        {
            //todo: implement redis for caching
            return File.ReadAllBytes(coverFileInfo.FullName);
        }
        
        foreach (string trackPath in trackPaths)
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                //todo: implement redis for caching
                return track.EmbeddedPictures.First().PictureData;
            }
        }

        return null;
    }

    public async Task<byte[]> GetArtistCoverByArtistIdAsync(Guid albumId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByArtistIdAsync(albumId);

        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory.Parent)
            .DistinctBy(dir => dir.Name)
            .SelectMany(dir => dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            .Select(dir => dir)
            .FirstOrDefault();

        if (coverFileInfo != null)
        {
            //todo: implement redis for caching
            return File.ReadAllBytes(coverFileInfo.FullName);
        }
        
        foreach (string trackPath in trackPaths)
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                //todo: implement redis for caching
                return track.EmbeddedPictures.First().PictureData;
            }
        }

        return null;
    }
}