namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;

public class PlaylistImportUserModel
{
    public Guid ImportId { get; set; }
    public Guid UserId { get; set; }
    public Guid PlaylistId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}