namespace MiniMediaSonicServer.Application.Models.Database;

public class UserPlayQueueTrackModel
{
    public Guid UserId { get; set; }
    public Guid TrackId { get; set; }
    public int Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}