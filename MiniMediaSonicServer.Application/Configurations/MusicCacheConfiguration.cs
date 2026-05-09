namespace MiniMediaSonicServer.Application.Configurations;

public class MusicCacheConfiguration
{
    public string? Path { get; set; }
    public string? DirectoryFormat { get; set; }
    public string DirectorySeparator { get; set; } = "_";
    public string? FileFormat { get; set; }
    public long? MaxCacheSize { get; set; }
    public long? MaxFileSize { get; set; }
    public bool? ExposeCachedFilePath { get; set; }
}