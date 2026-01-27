namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class CreatePlaylistRequest
{
    public Guid? PlaylistId { get; set; }
    public string Name { get; set; }
    public Guid SongId { get; set; }
}