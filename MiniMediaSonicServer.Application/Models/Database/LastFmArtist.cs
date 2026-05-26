using System.Collections.Concurrent;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.Database;

public class LastFmArtist
{
    public Guid ArtistId { get; set; }
    public string? LastFmId { get; set; }
    public string Name { get; set; }
    public bool OnTour { get; set; }
    public int StatsListeners { get; set; }
    public Guid? MusicBrainzId { get; set; }
    public string? BioContent { get; set; }
    public string? BioSummary { get; set; }
    public int? BioYearFormed { get; set; }
    public DateTime? BioPublished { get; set; }
    public string Uri { get; set; }
    public string ImageUri { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public List<Guid> SimilarArtistIds { get; set; } = new List<Guid>();
    public ConcurrentBag<ArtistID3> SimilarArtists { get; set; } = new ConcurrentBag<ArtistID3>();
}