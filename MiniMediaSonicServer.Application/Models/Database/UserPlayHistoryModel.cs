namespace MiniMediaSonicServer.Application.Models.Database;

public class UserPlayHistoryModel
{
    public Guid HistoryId { get; set; }
    public Guid UserId { get; set; }
    public Guid TrackId { get; set; }
    public bool Scrobble { get; set; }
    public DateTime? ScrobbleAt { get; set; }
    public string Artist { get; set; }
    public string Albumartist { get; set; }
    public string Artists { get; set; }
    public string Album { get; set; }
    public string Title { get; set; }
    public string Isrc { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}