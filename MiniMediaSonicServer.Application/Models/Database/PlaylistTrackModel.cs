namespace MiniMediaSonicServer.Application.Models.Database;

public class PlaylistTrackModel
{
    public required Guid PlaylistId { get; init; }
    public required Guid TrackId { get; init; }
    public required int TrackOrder { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Artist { get; init; }
    public required string AlbumArtist { get; init; }
    public required string Artists { get; init; }
    public required string Album { get; init; }
    public required string Title { get; init; }
    public required string ISRC { get; init; }
}