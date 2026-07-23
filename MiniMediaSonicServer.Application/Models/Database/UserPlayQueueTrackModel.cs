using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.Database;

public class UserPlayQueueTrackModel
{
    public required Guid UserId { get; init; }
    public required Guid TrackId { get; init; }
    public required int Index { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public TrackID3? Track { get; set; }
}