namespace MiniMediaSonicServer.Application.Models.Maloja;

public class ScrobbleTrackEntity
{
    public required List<string> Artists { get; init; }
    public required string Title { get; init; }
    public required string Album { get; init; }
    public required List<string> Albumartists { get; init; }
    public required int Duration { get; init; }
    public required int Length { get; init; }
    public required long Time { get; init; }
    public required string Key { get; init; }
}