namespace MiniMediaSonicServer.Application.Models.Database;

public class TrackBookmarkModel
{
    public Guid UserId { get; set; }
    public Guid TrackId { get; set; }
    public long Position { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Username { get; set; }
}