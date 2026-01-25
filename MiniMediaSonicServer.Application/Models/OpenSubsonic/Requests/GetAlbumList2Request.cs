namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class GetAlbumList2Request
{
    public string Type { get; set; }
    public int Size { get; set; }
    public int Offset { get; set; }
    public string FromYear { get; set; }
    public string ToYear { get; set; }
    public string Genre { get; set; }
    public int MusicFolderId { get; set; }
}