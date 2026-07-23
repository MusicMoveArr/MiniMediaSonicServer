namespace MiniMediaSonicServer.Application.Models.Database;

public class GenreCountModel
{
    public required string Genre { get; init; }
    public required int SongCount { get; init; }
    public required int AlbumCount { get; init; }
}