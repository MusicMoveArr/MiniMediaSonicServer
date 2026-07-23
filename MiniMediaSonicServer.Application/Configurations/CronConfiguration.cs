namespace MiniMediaSonicServer.Application.Configurations;

public class CronConfiguration
{
    public string? PlaylistImportCron { get; set; }
    public string? NavidromeSmartPlaylistRefreshCron { get; set; }
    public string? PlaylistFixTracksCron { get; set; }
    public string? PlayHistoryFixTracksCron { get; set; }
    public string? RatingsFixTracksCron { get; set; }
    public string? ReIndexSearchCron { get; set; }
    public string? ImportLastFmScrobblesCron { get; set; }
    public string? AutoLikeCron { get; set; }
    public string? ScrobblerCron { get; set; }
}