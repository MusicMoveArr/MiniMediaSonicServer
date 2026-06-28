namespace MiniMediaSonicServer.WebJob.Indexing.Application.Models;

public class SonicTrackModel
{
    public Guid TrackId { get; set; }
    public string Title { get; set; }
    public float Distance { get; set; }
}