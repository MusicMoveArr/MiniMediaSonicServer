namespace MiniMediaSonicServer.Application.Models.Database;

public class SonicTrackModel
{
    public Guid TrackId { get; set; }
    public string Title { get; set; }
    public string SourceTitle { get; set; }
    public float Distance { get; set; }
}