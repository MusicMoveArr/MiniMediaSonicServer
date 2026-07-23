using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.Database;

public class PlaylistModel
{
    public required Guid PlaylistId { get; init; }
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required bool Public { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string CoverArt { get; init; }
    public required bool ReadOnly { get; init; }
    
    //extra
    public required int SongCount { get; set; }
    public required int TotalDuration { get; init; }
    public required List<TrackID3> Tracks { get; set; } = [];
}