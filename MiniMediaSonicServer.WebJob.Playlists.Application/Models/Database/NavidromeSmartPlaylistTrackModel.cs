using Pgvector;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;

public class NavidromeSmartPlaylistTrackModel
{
    public Guid MetadataId { get; set; }
    public required string Title { get; set; }
    public required string Artist { get; set; }
    public required Vector Mood_Vector { get; set; }
}