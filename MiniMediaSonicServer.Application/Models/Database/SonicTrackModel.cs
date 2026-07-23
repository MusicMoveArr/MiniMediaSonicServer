using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.Database;

public class SonicTrackModel
{
    public required Guid TrackId { get; init; }
    public required string Title { get; init; }
    public required string SourceTitle { get; init; }
    public required float Distance { get; init; }
    public required TrackID3 Track { get; set; }
}