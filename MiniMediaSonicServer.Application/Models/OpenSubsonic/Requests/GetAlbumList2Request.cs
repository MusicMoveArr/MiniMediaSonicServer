using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetAlbumList2Request : SubsonicAuthModel
{
    public string Type { get; set; }
    public int Size { get; set; }
    public int Offset { get; set; }
    public string FromYear { get; set; }
    public string ToYear { get; set; }
    public string Genre { get; set; }
    public int MusicFolderId { get; set; }
}