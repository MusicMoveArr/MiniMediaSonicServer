namespace MiniMediaSonicServer.WebJob.Indexing.Application.Models;

public class IndexedTrackSonicModel
{
    public Guid TrackId { get; set; }
    public Guid RelatedTrackId { get; set; }
    public float Distance { get; set; }
}