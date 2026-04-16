using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetAlbumListRequest : SubsonicAuthModel
{
    public string Type { get; set; }
    public int Size { get; set; }
    public int Offset { get; set; }
    public int FromYear { get; set; }
    public int ToYear { get; set; }
    public string Genre { get; set; }
    public int MusicFolderId { get; set; }
}