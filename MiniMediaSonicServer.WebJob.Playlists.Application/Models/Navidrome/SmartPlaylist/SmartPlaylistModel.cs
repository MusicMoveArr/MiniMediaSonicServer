namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Navidrome.SmartPlaylist;

public class SmartPlaylistModel
{
    public string Name { get; set; }
    public string Comment { get; set; }
    public string Sort { get; set; }
    public string Order { get; set; }
    public int Limit { get; set; }
    public List<Operator> All { get; set; }
}