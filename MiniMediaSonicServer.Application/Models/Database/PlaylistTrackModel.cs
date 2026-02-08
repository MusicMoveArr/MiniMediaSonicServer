namespace MiniMediaSonicServer.Application.Models.Database;

public class PlaylistTrackModel
{
    public Guid PlaylistId { get; set; }
    public Guid TrackId { get; set; }
    public int TrackOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Artist { get; set; }
    public string AlbumArtist { get; set; }
    public string Artists { get; set; }
    public string Album { get; set; }
    public string Title { get; set; }
    public string ISRC { get; set; }
}