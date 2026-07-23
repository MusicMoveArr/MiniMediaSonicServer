using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Helpers;
using MiniMediaSonicServer.Application.Models;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class MusicCacheService
{
    private readonly MusicCacheConfiguration _musicCacheConfiguration;
    private readonly TrackRepository _trackRepository;

    public bool IsCachingEnabled => !string.IsNullOrWhiteSpace(_musicCacheConfiguration.Path) &&
                                    !string.IsNullOrWhiteSpace(_musicCacheConfiguration.FileFormat) &&
                                    !string.IsNullOrWhiteSpace(_musicCacheConfiguration.DirectoryFormat);
    
    public bool IsCachedFilePathExposed => IsCachingEnabled &&
                                           _musicCacheConfiguration.ExposeCachedFilePath == true;
    
    public MusicCacheService(
        IOptions<MusicCacheConfiguration> musicCacheConfiguration,
        TrackRepository trackRepository)
    {
        _musicCacheConfiguration = musicCacheConfiguration.Value;
        _trackRepository = trackRepository;
    }

    public async Task<string?> GetExposedCachedPathAsync(string sourceFilePath, Guid trackId)
    {
        if (!IsCachedFilePathExposed)
        {
            return sourceFilePath;
        }
        return await GetCachedOrOriginalFilePathAsync(sourceFilePath, trackId);
    }
    public string GetExposedCachedPath(string sourceFilePath, TrackID3 track)
    {
        if (_musicCacheConfiguration.ExposeCachedFilePath == false)
        {
            return sourceFilePath;
        }
        return GetCachedOrOriginalFilePath(sourceFilePath, track);
    }
    
    public async Task<string> GetCachedOrOriginalFilePathAsync(string sourceFilePath, Guid trackId)
    {
        if (!IsCachingEnabled)
        {
            return sourceFilePath;
        }

        var metadata = await _trackRepository.GetTrackByIdAsync(trackId, Guid.Empty);

        if (metadata == null)
        {
            return sourceFilePath;
        }
        
        MusicCacheTrackModel model = new MusicCacheTrackModel(sourceFilePath, metadata);
        string targetFilePath = GetTargetCacheFilePath(model);

        if (!string.IsNullOrWhiteSpace(targetFilePath) &&
            File.Exists(targetFilePath))
        {
            return targetFilePath;
        }
        return sourceFilePath;
    }
    
    public string GetCachedOrOriginalFilePath(string sourceFilePath, TrackID3 track)
    {
        if (!IsCachingEnabled)
        {
            return sourceFilePath;
        }
        
        MusicCacheTrackModel model = new MusicCacheTrackModel(sourceFilePath, track);
        string targetFilePath = GetTargetCacheFilePath(model);

        if (!string.IsNullOrWhiteSpace(targetFilePath) &&
            File.Exists(targetFilePath))
        {
            return targetFilePath;
        }
        return sourceFilePath;
    }

    public async Task SaveFileToCacheAsync(string sourceFilePath, Guid trackId)
    {
        FileInfo fileInfo = new FileInfo(sourceFilePath);
        if (!IsCachingEnabled ||
            !Directory.Exists(_musicCacheConfiguration.Path) ||
            (fileInfo.Length > _musicCacheConfiguration.MaxFileSize && _musicCacheConfiguration.MaxFileSize > 0) ||
            (fileInfo.Length > _musicCacheConfiguration.MaxCacheSize && _musicCacheConfiguration.MaxCacheSize > 0))
        {
            return;
        }

        var track = await _trackRepository.GetTrackByIdAsync(trackId, Guid.Empty);
        if (track == null)
        {
            return;
        }
        
        MusicCacheTrackModel model = new MusicCacheTrackModel(sourceFilePath, track);
        string targetFilePath = GetTargetCacheFilePath(model);
        
        FileInfo targetFileInfo = new FileInfo(targetFilePath);
        DirectoryInfo? targetDirectory = targetFileInfo.Directory;

        if (targetDirectory?.Exists == false)
        {
            Directory.CreateDirectory(targetDirectory.FullName);
        }

        if (!targetFileInfo.Exists ||
            targetFileInfo.Length != fileInfo.Length)
        {
            File.Copy(sourceFilePath, targetFilePath, true);
            File.SetLastAccessTime(targetFilePath, DateTime.Now);

            CleanupCache();
        }
    }

    private string GetTargetCacheDirectoryPath(MusicCacheTrackModel model)
    {
        string directoryFormat = ArtistHelper.GetFormatName(model, _musicCacheConfiguration.DirectoryFormat, _musicCacheConfiguration.DirectorySeparator);
        return Path.Join(_musicCacheConfiguration.Path, directoryFormat);
    }

    private string GetTargetCacheFilePath(MusicCacheTrackModel model)
    {
        string fileFormat = ArtistHelper.GetFormatName(model, _musicCacheConfiguration.FileFormat, _musicCacheConfiguration.DirectorySeparator);
        string targetDirectory = GetTargetCacheDirectoryPath(model);
        return Path.Join(targetDirectory, fileFormat) + model.FileExtension;
    }

    public void CleanupCache()
    {
        long cacheUsage = TotalCacheUsage();
        if (string.IsNullOrWhiteSpace(_musicCacheConfiguration.Path) ||
            cacheUsage < _musicCacheConfiguration.MaxCacheSize ||
            _musicCacheConfiguration.MaxCacheSize <= 0)
        {
            return;
        }

        foreach (var file in new DirectoryInfo(_musicCacheConfiguration.Path)
                            .EnumerateFiles("*", SearchOption.AllDirectories)
                            .OrderBy(f => f.LastAccessTime))
        {
            try
            {
                long fileSize = file.Length;
                file.Delete();
                cacheUsage -= fileSize;
                if (cacheUsage < _musicCacheConfiguration.MaxCacheSize)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to delete cached track '{file.FullName}' '{e.Message}'");
            }
        }
    }

    private long TotalCacheUsage()
    {
        if (string.IsNullOrWhiteSpace(_musicCacheConfiguration.Path) ||
            !Directory.Exists(_musicCacheConfiguration.Path))
        {
            return -1;
        }
        
        return new DirectoryInfo(_musicCacheConfiguration.Path)
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .AsParallel()
            .Sum(file => file.Length);
    }
}