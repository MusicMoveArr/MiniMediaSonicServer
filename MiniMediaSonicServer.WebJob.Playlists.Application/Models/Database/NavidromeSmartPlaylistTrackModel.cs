using Pgvector;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;

public class NavidromeSmartPlaylistTrackModel
{
    public Guid MetadataId { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public Vector Mood_Vector { get; set; }
}