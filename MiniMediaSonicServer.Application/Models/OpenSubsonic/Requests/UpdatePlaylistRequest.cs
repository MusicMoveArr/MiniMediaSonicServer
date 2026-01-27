namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class UpdatePlaylistRequest
{
    public Guid PlaylistId { get; set; }
    public string? Name { get; set; }
    public string? Comment { get; set; }
    public bool? Public { get; set; }
    public List<Guid>? SongIdToAdd { get; set; }
    public List<Guid>? SongIndexToRemove { get; set; }
}