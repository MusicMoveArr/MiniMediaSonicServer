using MiniMediaSonicServer.Application.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            .Select(dir => dir)
            .FirstOrDefault();

        if (coverFileInfo != null)
        {
            //todo: implement redis for caching
            return File.ReadAllBytes(coverFileInfo.FullName);
        }
        
        foreach (string trackPath in trackPaths.Where(file => File.Exists(file)))
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
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            .Select(dir => dir)
            .FirstOrDefault();

        if (coverFileInfo != null)
        {
            //todo: implement redis for caching
            return File.ReadAllBytes(coverFileInfo.FullName);
        }
        
        foreach (string trackPath in trackPaths.Where(file => File.Exists(file)))
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

    public async Task<byte[]> GetPlaylistCoverByIdAsync(Guid playlistId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByPlaylistIdAsync(playlistId);

        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory)
            .DistinctBy(dir => dir.Name)
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            .Select(dir => dir)
            .DistinctBy(dir => dir.FullName)
            .Take(4)
            .ToList();

        if (coverFileInfo.Count < 4)
        {
            //todo: implement redis for caching
            return File.ReadAllBytes(coverFileInfo.First().FullName);
        }

        List<Image> covers = coverFileInfo
            .Select(cover => Image.Load(cover.FullName))
            .ToList();

        foreach (Image cover in covers)
        {
            cover.Mutate(ctx =>
            {
                ctx.Resize(new Size(200, 200));
            });
        }
            
        using var img = new Image<Rgba32>(400, 400);

        img.Mutate(ctx =>
        {
            ctx.DrawImage(covers[0], new Point(0, 0), PixelColorBlendingMode.Normal, 1f);
            ctx.DrawImage(covers[1], new Point(200, 0), PixelColorBlendingMode.Normal, 1f);
            ctx.DrawImage(covers[2], new Point(0, 200), PixelColorBlendingMode.Normal, 1f);
            ctx.DrawImage(covers[3], new Point(200, 200), PixelColorBlendingMode.Normal, 1f);
        });

        using (MemoryStream stream = new MemoryStream())
        {
            img.Save(stream, new JpegEncoder());

            foreach (Image cover in covers)
            {
                cover.Dispose();
            }
            
            return stream.ToArray();
        }
    }
}