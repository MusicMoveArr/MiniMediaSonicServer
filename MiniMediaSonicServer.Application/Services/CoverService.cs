using MiniMediaSonicServer.Application.Models;
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
    private readonly EnumerationOptions _enumerationOptions = new EnumerationOptions
    {
        IgnoreInaccessible = true,
        RecurseSubdirectories = false,
        AttributesToSkip = FileAttributes.System | FileAttributes.Hidden
    };

    private readonly string[] jpegWebpExtentions = [".jpg", ".webp"];
    private const string ResizedContentType = "image/jpeg";
    private const string WebpContentType = "image/webp";
    private const string webpExtension = ".webp";

    public CoverService(TrackCoverRepository trackCoverRepository)
    {
        _trackCoverRepository = trackCoverRepository;
    }

    public async Task<CoverArtModel?> GetAlbumCoverByTrackIdAsync(Guid trackId)
    {
        string? trackPath = await _trackCoverRepository.GetTrackPathByTrackIdAsync(trackId);
        
        if(string.IsNullOrWhiteSpace(trackPath))
        {
            return null;
        }
        
        FileInfo trackFileInfo = new FileInfo(trackPath);
        
        if (trackFileInfo.Directory?.Exists == true)
        {
            var coverFileInfo = trackFileInfo.Directory
                .GetFiles("*.*", _enumerationOptions)
                .Where(f => jpegWebpExtentions.Any(ext => f.Name.EndsWith(ext)))
                .Select(f => new
                {
                    File = f,
                    Order = f.Name.EndsWith(".webp") ? 0 : 1
                })
                .OrderBy(f => f.Order)
                .Select(f => f.File)
                .FirstOrDefault();

            if (coverFileInfo != null)
            {
                if (coverFileInfo.Name.EndsWith(webpExtension))
                {
                    return new CoverArtModel(File.ReadAllBytes(coverFileInfo.FullName), WebpContentType);
                }
                
                byte[]? coverData = GetBytesOfResizedImage(coverFileInfo.FullName);
                if (coverData != null)
                {
                    return new CoverArtModel(coverData, ResizedContentType);
                }
            }
        }
        
        if(trackFileInfo.Exists)
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                byte[]? coverData = GetBytesOfResizedImage(track.EmbeddedPictures.First().PictureData, trackPath);
                return new CoverArtModel(coverData, ResizedContentType);
            }
        }

        return null;
    }

    public async Task<CoverArtModel?> GetAlbumCoverByAlbumIdAsync(Guid albumId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByAlbumIdAsync(albumId);
        
        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory)
            .DistinctBy(dir => dir.Name)
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.*", _enumerationOptions)
                .Where(f => jpegWebpExtentions.Any(ext => f.Name.EndsWith(ext))))
            .Select(f => new
            {
                File = f,
                Order = f.Name.EndsWith(".webp") ? 0 : 1
            })
            .OrderBy(f => f.Order)
            .Select(f => f.File)
            .FirstOrDefault();
        
        if (coverFileInfo != null)
        {
            if (coverFileInfo.Name.EndsWith(webpExtension))
            {
                return new CoverArtModel(File.ReadAllBytes(coverFileInfo.FullName), WebpContentType);
            }
            
            byte[]? coverData = GetBytesOfResizedImage(coverFileInfo.FullName);
            if (coverData != null)
            {
                return new CoverArtModel(coverData, ResizedContentType);
            }
        }
        
        foreach (string trackPath in trackPaths.Where(file => File.Exists(file)))
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                byte[]? coverData = GetBytesOfResizedImage(track.EmbeddedPictures.First().PictureData, trackPath);
                if (coverData != null)
                {
                    return new CoverArtModel(coverData, ResizedContentType);
                }
            }
        }

        return null;
    }

    public async Task<CoverArtModel?> GetArtistCoverByArtistIdAsync(Guid artistId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByArtistIdAsync(artistId);
        
        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory)
            .DistinctBy(dir => dir.Name)
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.*", _enumerationOptions)
                .Where(f => jpegWebpExtentions.Any(ext => f.Name.EndsWith(ext))))
            .Select(f => new
            {
                File = f,
                Order = f.Name.EndsWith(".webp") ? 0 : 1
            })
            .OrderBy(f => f.Order)
            .Select(f => f.File)
            .FirstOrDefault();

        if (coverFileInfo != null)
        {
            if (coverFileInfo.Name.EndsWith(webpExtension))
            {
                return new CoverArtModel(File.ReadAllBytes(coverFileInfo.FullName), WebpContentType);
            }
            
            byte[]? coverData = GetBytesOfResizedImage(coverFileInfo.FullName);
            if (coverData != null)
            {
                return new CoverArtModel(coverData, ResizedContentType);
            }
        }
        
        foreach (string trackPath in trackPaths.Where(file => File.Exists(file)))
        {
            ATL.Track track = new ATL.Track(trackPath);
            if (track.EmbeddedPictures.Any())
            {
                byte[]? coverData = GetBytesOfResizedImage(track.EmbeddedPictures.First().PictureData, trackPath);
                if (coverData != null)
                {
                    return new CoverArtModel(coverData, ResizedContentType);
                }
            }
        }
        return null;
    }

    public async Task<CoverArtModel?> GetPlaylistCoverByIdAsync(Guid playlistId)
    {
        List<string> trackPaths = await _trackCoverRepository.GetTrackPathByPlaylistIdAsync(playlistId);
        
        var coverFileInfo = trackPaths
            .Select(path => new FileInfo(path).Directory)
            .DistinctBy(dir => dir.Name)
            .Where(dir => dir.Exists)
            .SelectMany(dir => dir.GetFiles("*.jpg", _enumerationOptions))
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
            byte[]? coverData = GetBytesOfResizedImage(coverFileInfo.First().FullName);
            return new CoverArtModel(coverData, ResizedContentType);
        }

        List<Image> covers = coverFileInfo
            .Select(cover => TryLoadImage(cover.FullName))
            .Where(cover => cover != null)
            .ToList()!;

        if (covers.Count < 4)
        {
            foreach (Image cover in covers)
            {
                cover.Dispose();
            }
            byte[]? coverData = GetBytesOfResizedImage(coverFileInfo.First().FullName);
            return new CoverArtModel(coverData, ResizedContentType);
        }

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
            
            return new CoverArtModel(stream.ToArray(), ResizedContentType);
        }
    }

    private byte[]? GetBytesOfResizedImage(string path)
    {
        using var image = TryLoadImage(path);
        
        if (image == null)
        {
            return null;
        }
        
        using MemoryStream stream = new MemoryStream();
        image.Mutate(ctx =>
        {
            ctx.Resize(this.DefaultCoverSize);
        });
        image.Save(stream, new JpegEncoder());
        return stream.ToArray();
    }
    private byte[]? GetBytesOfResizedImage(byte[]? imageData, string path)
    {
        if (imageData == null)
        {
            return null;
        }
        
        using var image = TryLoadImage(imageData, path);

        if (image == null)
        {
            return null;
        }
        
        using MemoryStream stream = new MemoryStream();
        image.Mutate(ctx =>
        {
            ctx.Resize(this.DefaultCoverSize);
        });
        image.Save(stream, new JpegEncoder());
        return stream.ToArray();
    }

    private Image? TryLoadImage(string path)
    {
        try
        {
            return Image.Load(path);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cover possibly corrupt, path: '{path}', error: '{e.Message}'");
        }
        return null;
    }
    private Image? TryLoadImage(byte[] imageData, string path)
    {
        try
        {
            return Image.Load(imageData);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cover possibly corrupt, image data from memory, path: '{path}', error: '{e.Message}'");
        }
        return null;
    }
}