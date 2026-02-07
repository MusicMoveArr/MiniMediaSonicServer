namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;

public class PlaylistImportModel
{
    public Guid ImportId { get; set; }
    public string Path { get; set; }
    public bool IsGlobal { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}