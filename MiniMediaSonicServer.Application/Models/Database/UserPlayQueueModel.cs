namespace MiniMediaSonicServer.Application.Models.Database;

public class UserPlayQueueModel
{
    public required Guid UserId { get; init; }
    public required Guid CurrentTrackId { get; init; }
    public required long TrackPosition { get; init; }
    public required string Username { get; init; }
    public required string UpdatedByAppName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required List<UserPlayQueueTrackModel> Tracks { get; set; } = [];
}