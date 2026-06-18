namespace MiniMediaSonicServer.Application.Models.Database;

public class RatedTrackModel
{
    public Guid UserId { get; set; }
    public Guid TrackId { get; set; }
    public int Rating { get; set; }
    public bool Starred { get; set; }
    public string Artist { get; set; }
    public string AlbumArtist { get; set; }
    public string Artists { get; set; }
    public string Album { get; set; }
    public string Title { get; set; }
    public string ISRC { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? StarredAt { get; set; }
    public DateTime? RatedAt { get; set; }
    public DateTime? UnStarredAt { get; set; }
}