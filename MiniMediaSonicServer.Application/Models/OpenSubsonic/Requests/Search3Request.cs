namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class Search3Request
{
    public string Query { get; set; }
    public int ArtistCount { get; set; }
    public int ArtistOffset { get; set; }
    public int AlbumCount { get; set; }
    public int AlbumOffset { get; set; }
    public int SongCount { get; set; }
    public int SongOffset { get; set; }
    public int MusicFolderId { get; set; }
}