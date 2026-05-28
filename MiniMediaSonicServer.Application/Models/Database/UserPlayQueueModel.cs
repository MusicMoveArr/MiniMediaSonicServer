namespace MiniMediaSonicServer.Application.Models.Database;

public class UserPlayQueueModel
{
    public Guid UserId { get; set; }
    public Guid CurrentTrackId { get; set; }
    public long TrackPosition { get; set; }
    public string Username { get; set; }
    public string UpdatedByAppName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserPlayQueueTrackModel> Tracks { get; set; } = new List<UserPlayQueueTrackModel>();
}