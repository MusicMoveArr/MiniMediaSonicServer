namespace MiniMediaSonicServer.Application.Models.Database;

public class TrackBookmarkModel
{
    public required Guid UserId { get; init; }
    public required Guid TrackId { get; init; }
    public required long Position { get; init; }
    public required string Comment { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string Username { get; init; }
}