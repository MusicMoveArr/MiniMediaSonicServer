namespace MiniMediaSonicServer.Application.Models.Database;

public class RatedTrackModel
{
    public required Guid UserId { get; init; }
    public required Guid TrackId { get; init; }
    public required int Rating { get; init; }
    public required bool Starred { get; init; }
    public required string Artist { get; init; }
    public required string AlbumArtist { get; init; }
    public required string Artists { get; init; }
    public required string Album { get; init; }
    public required string Title { get; init; }
    public required string ISRC { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? StarredAt { get; init; }
    public DateTime? RatedAt { get; init; }
    public DateTime? UnStarredAt { get; init; }
}