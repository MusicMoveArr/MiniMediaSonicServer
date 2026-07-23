using System.Collections.Concurrent;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.Database;

public class LastFmArtist
{
    public required Guid ArtistId { get; init; }
    public required string? LastFmId { get; init; }
    public required string Name { get; init; }
    public required bool OnTour { get; init; }
    public required int StatsListeners { get; init; }
    public required Guid? MusicBrainzId { get; init; }
    public required string? BioContent { get; init; }
    public required string? BioSummary { get; init; }
    public required int? BioYearFormed { get; init; }
    public required DateTime? BioPublished { get; init; }
    public required string Uri { get; init; }
    public required string ImageUri { get; init; }
    public required DateTime? LastSyncTime { get; init; }
    public required List<Guid> SimilarArtistIds { get; set; } = [];
    public required ConcurrentBag<ArtistID3> SimilarArtists { get; init; } = [];
}