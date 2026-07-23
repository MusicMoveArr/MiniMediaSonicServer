namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Navidrome.SmartPlaylist;

public class SmartPlaylistModel
{
    public required string Name { get; set; }
    public required string Comment { get; set; }
    public required string Sort { get; set; }
    public required string Order { get; set; }
    public int Limit { get; set; }
    public SonicSort? SonicSort { get; set; }
    public List<Operator> All { get; set; } = [];
}