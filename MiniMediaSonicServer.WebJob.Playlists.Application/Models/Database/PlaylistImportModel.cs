namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;

public class PlaylistImportModel
{
    public required Guid ImportId { get; init; }
    public required string Path { get; init; }
    public required bool IsGlobal { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}