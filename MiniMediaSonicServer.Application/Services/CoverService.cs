using MiniMediaSonicServer.Application.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MiniMediaSonicServer.Application.Services;

public class CoverService
{
    private readonly TrackCoverRepository _trackCoverRepository;
    private readonly Size DefaultCoverSize = new Size(400, 400);

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
            return GetBytesOfResizedImage(coverFileInfo.FullName);
        }
        
        foreach (string trackPath in trackPaths.Where(file => File.Exists(file)))
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                return GetBytesOfResizedImage(track.EmbeddedPictures.First().PictureData);
            }
        }

        return null;
    }

    public async Task<byte[]> GetArtistCoverByArtistIdAsync(Guid artistId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByArtistIdAsync(artistId);

        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory.Parent)
            .DistinctBy(dir => dir.Name)
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            .Select(dir => dir)
            .FirstOrDefault();

        if (coverFileInfo != null)
        {
            return GetBytesOfResizedImage(coverFileInfo.FullName);
        }
        
        foreach (string trackPath in trackPaths.Where(file => File.Exists(file)))
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                return GetBytesOfResizedImage(track.EmbeddedPictures.First().PictureData);
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

        if (!coverFileInfo.Any())
        {
            return null;
        }

        if (coverFileInfo.Count < 4)
        {
            return GetBytesOfResizedImage(coverFileInfo.First().FullName);
        }

        List<Image> covers = coverFileInfo
            .Select(cover => Image.Load(cover.FullName))
            .ToList();

        int singleImageWidth = this.DefaultCoverSize.Width / 2;
        int singleImageHeight = this.DefaultCoverSize.Height / 2;
        
        foreach (Image cover in covers)
        {
            cover.Mutate(ctx =>
            {
                ctx.Resize(new Size(singleImageWidth, singleImageHeight));
            });
        }
            
        using var img = new Image<Rgba32>(this.DefaultCoverSize.Width, this.DefaultCoverSize.Height);

        img.Mutate(ctx =>
        {
            ctx.DrawImage(covers[0], new Point(0, 0), PixelColorBlendingMode.Normal, 1f);
            ctx.DrawImage(covers[1], new Point(singleImageWidth, 0), PixelColorBlendingMode.Normal, 1f);
            ctx.DrawImage(covers[2], new Point(0, singleImageHeight), PixelColorBlendingMode.Normal, 1f);
            ctx.DrawImage(covers[3], new Point(singleImageWidth, singleImageHeight), PixelColorBlendingMode.Normal, 1f);
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

    private byte[] GetBytesOfResizedImage(string path)
    {
        using var image = Image.Load(path);
        using MemoryStream stream = new MemoryStream();
        image.Mutate(ctx =>
        {
            ctx.Resize(this.DefaultCoverSize);
        });
        image.Save(stream, new JpegEncoder());
        return stream.ToArray();
    }
    private byte[]? GetBytesOfResizedImage(byte[]? imageData)
    {
        if (imageData == null)
        {
            return null;
        }
        
        using var image = Image.Load(imageData);
        using MemoryStream stream = new MemoryStream();
        image.Mutate(ctx =>
        {
            ctx.Resize(this.DefaultCoverSize);
        });
        image.Save(stream, new JpegEncoder());
        return stream.ToArray();
    }
}