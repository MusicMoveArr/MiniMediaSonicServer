namespace MiniMediaSonicServer.Application.Models.Maloja;

public class ScrobbleTrackEntity
{
    public List<string> Artists { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public List<string> Albumartists { get; set; }
    public int Duration { get; set; }
    public int Length { get; set; }
    public string Key { get; set; }
}