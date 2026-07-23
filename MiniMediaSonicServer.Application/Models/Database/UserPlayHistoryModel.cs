namespace MiniMediaSonicServer.Application.Models.Database;

public class UserPlayHistoryModel
{
    public required Guid HistoryId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid TrackId { get; init; }
    public required bool Scrobble { get; init; }
    public DateTime? ScrobbleAt { get; init; }
    public required string Artist { get; init; }
    public required string Albumartist { get; init; }
    public required string Artists { get; init; }
    public required string Album { get; init; }
    public required string Title { get; init; }
    public required string Isrc { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string ImportedBy { get; init; }
    public required long PlayOffset { get; init; }
}