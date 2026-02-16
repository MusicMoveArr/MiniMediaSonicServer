using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.Database;

public class PlaylistModel
{
    public Guid PlaylistId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public bool Public { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool ReadOnly { get; set; }
    
    //extra
    public int SongCount { get; set; }
    public int TotalDuration { get; set; }
    public List<TrackID3> Tracks { get; set; } = new List<TrackID3>();
}